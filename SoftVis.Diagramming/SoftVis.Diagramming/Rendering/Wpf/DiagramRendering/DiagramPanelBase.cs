using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Codartis.SoftVis.Common;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Rendering.Wpf.Common;
using Codartis.SoftVis.Rendering.Wpf.DiagramRendering.ShapeControls;

namespace Codartis.SoftVis.Rendering.Wpf.DiagramRendering
{
    /// <summary>
    /// Base class for panels that render diagrams by creating and arranging diagram shapes.
    /// </summary>
    internal abstract class DiagramPanelBase : Panel
    {
        protected readonly Map<DiagramShape, DiagramShapeControlBase> DiagramShapeToControlMap =
            new Map<DiagramShape, DiagramShapeControlBase>();

        protected readonly Map<DiagramShapeControlBase, DiagramShape> ControlToDiagramShapeMap =
            new Map<DiagramShapeControlBase, DiagramShape>();

        private Rect _contentRect;

        public static readonly DependencyProperty DiagramProperty =
            DependencyProperty.Register("Diagram", typeof(Diagram), typeof(DiagramPanelBase),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, Diagram_PropertyChanged));

        private static readonly Duration ShapeEnterAnimationDuration = new Duration(TimeSpan.FromMilliseconds(200));
        private static readonly Duration ShapeExitAnimationDuration = ShapeEnterAnimationDuration;
        private static readonly Duration ShapeMoveAnimationDuration = ShapeEnterAnimationDuration;

        public Diagram Diagram
        {
            get { return (Diagram)GetValue(DiagramProperty); }
            set { SetValue(DiagramProperty, value); }
        }

        private static void Diagram_PropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var diagramPanel = (DiagramPanelBase)obj;
            var diagram = (Diagram)e.NewValue;
            diagramPanel.AddDiagram(diagram);

            diagram.ShapeAdded += diagramPanel.OnShapeAdded;
            diagram.ShapeModified += diagramPanel.OnShapeModified;
            diagram.ShapeRemoved += diagramPanel.OnShapeRemoved;
            diagram.Cleared += diagramPanel.OnDiagramCleared;
        }

        protected Rect ContentRect
        {
            get { return _contentRect == Rect.Empty ? new Rect(0, 0, 0, 0) : _contentRect; }
            private set { _contentRect = value; }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (var child in Children.OfType<DiagramShapeControlBase>())
                child.Measure(availableSize);

            ContentRect = Children.OfType<DiagramShapeControlBase>().Select(i => i.Rect).Union();
            return ContentRect.Size;
        }

        private void AddDiagram(Diagram diagram)
        {
            AddAllGraphElements(diagram);
        }

        private void OnShapeAdded(object sender, DiagramShape shape)
        {
            if (DiagramShapeToControlMap.Contains(shape))
                return;

            if (shape is DiagramNode)
                CreateDiagramNodeControl((DiagramNode)shape);
            else if (shape is DiagramConnector)
                CreateDiagramConnectorControl((DiagramConnector)shape);
        }

        private void OnShapeModified(object sender, DiagramShape shape)
        {
            var control = DiagramShapeToControlMap.Get(shape);
            if (control == null)
                return;


            var diagramNodeControl = control as DiagramNodeControl;
            if (diagramNodeControl != null)
            {
                var rect = ((DiagramNode) shape).Rect.ToWpf();
                diagramNodeControl.Size = rect.Size;
                if (diagramNodeControl.Position.IsExtreme())
                    diagramNodeControl.Position = rect.Location;
                else
                    AnimateNodeMove(diagramNodeControl, rect.Location);
            }

            var diagramConnectorControl = control as DiagramConnectorControl;
            if (diagramConnectorControl != null)
            {
                var rect = CreateRect((DiagramConnector)shape);
                diagramConnectorControl.Size = rect.Size;
                diagramConnectorControl.Position = rect.Location;
            }
        }

        private static Rect CreateRect(DiagramConnector diagramConnector)
        {
            var rectUnion = new[]
            {
                diagramConnector.Source.Rect.ToWpf(),
                diagramConnector.Target.Rect.ToWpf()
            }.Union();

            var routePoints = diagramConnector.RoutePoints.Select(j => j.ToWpf());
            foreach (var routePoint in routePoints)
                rectUnion.Union(routePoint);

            return rectUnion;
        }

        private void OnShapeRemoved(object sender, DiagramShape shape)
        {
            if (!DiagramShapeToControlMap.Contains(shape))
                return;

            RemoveDiagramShapeControl(shape);
        }

        private void OnDiagramCleared(object sender, EventArgs e)
        {
            Children.Clear();
            DiagramShapeToControlMap.Clear();
            ControlToDiagramShapeMap.Clear();
        }

        private void AddAllGraphElements(Diagram diagram)
        {
            foreach (var node in diagram.Nodes)
                CreateDiagramNodeControl(node);

            foreach (var connector in diagram.Connectors)
                CreateDiagramConnectorControl(connector);
        }

        private void CreateDiagramNodeControl(DiagramNode diagramNode)
        {
            var control = new DiagramNodeControl { DataContext = diagramNode };
            control.PreviewMouseDoubleClick += OnDiagramNodeDoubleClicked;
            control.PreviewMouseLeftButtonDown += OnDiagramNodeLeftButtonDown;

            DiagramShapeToControlMap.Set(diagramNode, control);
            ControlToDiagramShapeMap.Set(control, diagramNode);
            Children.Add(control);
            AnimateShapeEnter(control);
            OnShapeModified(this, diagramNode);
        }

        private void CreateDiagramConnectorControl(DiagramConnector diagramConnector)
        {
            var control = new DiagramConnectorControl { DataContext = diagramConnector };
            DiagramShapeToControlMap.Set(diagramConnector, control);
            ControlToDiagramShapeMap.Set(control, diagramConnector);
            Children.Add(control);
            AnimateShapeEnter(control);
        }

        private void RemoveDiagramShapeControl(DiagramShape diagramShape)
        {
            var control = DiagramShapeToControlMap.Get(diagramShape);
            AnimateShapeExit(control);
            Children.Remove(control);
            DiagramShapeToControlMap.Remove(diagramShape);
            ControlToDiagramShapeMap.Remove(control);
        }

        private void OnDiagramNodeLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var senderDiagramNode = sender as DiagramNodeControl;
            if (senderDiagramNode == null || !ControlToDiagramShapeMap.Contains(senderDiagramNode))
                return;

            Diagram.OnShapeSelected(ControlToDiagramShapeMap.Get(senderDiagramNode));
        }

        private void OnDiagramNodeDoubleClicked(object sender, MouseButtonEventArgs e)
        {
            var senderDiagramNode = sender as DiagramNodeControl;
            if (senderDiagramNode == null || !ControlToDiagramShapeMap.Contains(senderDiagramNode))
                return;

            Diagram.OnShapeActivated(ControlToDiagramShapeMap.Get(senderDiagramNode));
            e.Handled = true;
        }

        private static void AnimateShapeEnter(DiagramShapeControlBase control)
        {
            var animation = new DoubleAnimation(0, 1, ShapeEnterAnimationDuration);
            control.BeginAnimation(DiagramShapeControlBase.ScaleProperty, animation);
        }

        private static void AnimateShapeExit(DiagramShapeControlBase control)
        {
            var animation = new DoubleAnimation(1, 0, ShapeExitAnimationDuration);
            control.BeginAnimation(DiagramShapeControlBase.ScaleProperty, animation);
        }

        private static void AnimateNodeMove(DiagramNodeControl control, Point toPosition)
        {
            var animation = new PointAnimation(toPosition, ShapeMoveAnimationDuration);
            control.BeginAnimation(DiagramShapeControlBase.PositionProperty, animation);
        }
    }
}
