using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Rendering.Extensibility;
using Codartis.SoftVis.Rendering.Wpf.Common;
using Codartis.SoftVis.Rendering.Wpf.Common.HitTesting;
using Codartis.SoftVis.Rendering.Wpf.DiagramRendering.Shapes;
using Codartis.SoftVis.Rendering.Wpf.DiagramRendering.Viewport.Modification.MiniButtons;

namespace Codartis.SoftVis.Rendering.Wpf.DiagramRendering.Viewport
{
    /// <summary>
    /// Renders the visible part of a Diagram.
    /// </summary>
    internal class DiagramViewportPanel : DiagramPanelBase, IDiagramViewport
    {
        private readonly List<MiniButtonBase> _miniButtons;

        private double _zoom;
        private Size _sizeInScreenSpace;
        private Point _centerInDiagramSpace;

        public Rect ViewportInScreenSpace { get; private set; }
        public Rect ViewportInDiagramSpace { get; private set; }
        private Transform DiagramSpaceToScreenSpace { get; set; }

        public static readonly DependencyProperty DiagramBehaviourProviderProperty =
            DependencyProperty.Register("DiagramBehaviourProvider", typeof(IDiagramBehaviourProvider),
                typeof(DiagramViewportPanel));

        public static readonly DependencyProperty DiagramHitTesterProperty =
            DependencyProperty.Register("DiagramHitTester", typeof(IHitTester),
                typeof(DiagramViewportPanel));

        public static readonly DependencyProperty MinZoomProperty =
            DependencyProperty.Register("MinZoom", typeof(double), typeof(DiagramViewportPanel));

        public static readonly DependencyProperty MaxZoomProperty =
            DependencyProperty.Register("MaxZoom", typeof(double), typeof(DiagramViewportPanel));

        public DiagramViewportPanel()
        {
            _miniButtons = new List<MiniButtonBase>();

            _zoom = 1.0;
            _sizeInScreenSpace = Size.Empty;
            _centerInDiagramSpace = new Point(0, 0);

            RecalculateCachedValues();
        }

        public IDiagramBehaviourProvider DiagramBehaviourProvider
        {
            get { return (IDiagramBehaviourProvider)GetValue(DiagramBehaviourProviderProperty); }
            set { SetValue(DiagramBehaviourProviderProperty, value); }
        }

        public IHitTester DiagramHitTester
        {
            get { return (IHitTester)GetValue(DiagramHitTesterProperty); }
            set { SetValue(DiagramHitTesterProperty, value); }
        }

        public double MinZoom
        {
            get { return (double)GetValue(MinZoomProperty); }
            set { SetValue(MinZoomProperty, value); }
        }

        public double MaxZoom
        {
            get { return (double)GetValue(MaxZoomProperty); }
            set { SetValue(MaxZoomProperty, value); }
        }

        public Rect ContentInDiagramSpace => ContentRect;

        public double Zoom
        {
            get { return _zoom; }
            private set
            {
                lock (this)
                {
                    _zoom = value;
                    RecalculateCachedValues();
                }
            }
        }

        public Size SizeInScreenSpace
        {
            get { return _sizeInScreenSpace; }
            private set
            {
                lock (this)
                {
                    _sizeInScreenSpace = value;
                    RecalculateCachedValues();
                }
            }
        }

        public Point CenterInDiagramSpace
        {
            get { return _centerInDiagramSpace; }
            private set
            {
                lock (this)
                {
                    _centerInDiagramSpace = value;
                    RecalculateCachedValues();
                }
            }
        }

        public void MoveCenterInDiagramSpace(Point newCenterInDiagramSpace)
        {
            CenterInDiagramSpace = newCenterInDiagramSpace;
        }

        public void MoveCenterInScreenSpace(Point newCenterInScreenSpace)
        {
            CenterInDiagramSpace = ProjectPointIntoDiagramSpace(newCenterInScreenSpace);
        }

        public void ResizeInScreenSpace(Size newSizeInScreenSpace)
        {
            SizeInScreenSpace = newSizeInScreenSpace;
        }

        public void ZoomTo(double newZoom)
        {
            Zoom = newZoom;
        }

        public void ZoomWithCenterInScreenSpace(double newZoom, Point zoomCenterInScreenSpace)
        {
            var zoomCenterInDiagramSpace = ProjectPointIntoDiagramSpace(zoomCenterInScreenSpace);
            CenterInDiagramSpace = (CenterInDiagramSpace - zoomCenterInDiagramSpace) * (Zoom / newZoom) + zoomCenterInDiagramSpace;
            Zoom = newZoom;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            base.MeasureOverride(availableSize);

            // We want to fill all available space.
            return Size.Empty;
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            foreach (var child in Children.OfType<DiagramShapeControlBase>())
                ArrangeChildControl(child);

            return arrangeSize;
        }

        protected override DiagramNodeControl CreateDiagramNodeControl(DiagramNode diagramNode)
        {
            var control = base.CreateDiagramNodeControl(diagramNode);

            control.MouseEnter += OnDiagramNodeMouseEnterOrLeave;
            control.MouseLeave += OnDiagramNodeMouseEnterOrLeave;
            control.PreviewMouseDoubleClick += OnDiagramNodeDoubleClicked;
            control.PreviewMouseLeftButtonDown += OnDiagramNodeLeftButtonDown;

            return control;
        }

        private void ArrangeChildControl(DiagramShapeControlBase child)
        {
            child.Arrange(new Rect(child.DesiredSize));
            child.RenderTransform = CreateTransformForChild(child.Position, child.Size, child.Scale);
        }

        private Transform CreateTransformForChild(Point position, Size size, double scale)
        {
            var transform = new TransformGroup();
            transform.Children.Add(new TranslateTransform(size.Width / -2, size.Height / -2));
            transform.Children.Add(new ScaleTransform(scale, scale));
            transform.Children.Add(new TranslateTransform(size.Width / 2, size.Height / 2));
            transform.Children.Add(new TranslateTransform(position.X, position.Y));
            transform.Children.Add(DiagramSpaceToScreenSpace);
            return transform;
        }

        private void RecalculateCachedValues()
        {
            ViewportInScreenSpace = new Rect(SizeInScreenSpace);
            ViewportInDiagramSpace = ProjectViewportIntoDiagramSpace();
            DiagramSpaceToScreenSpace = CalculateTransformForDiagramSpaceToScreenSpace(ViewportInDiagramSpace);
        }

        private Point ProjectPointIntoDiagramSpace(Point pointInScreenSpace)
        {
            var screenCenter = new Rect(SizeInScreenSpace).GetCenter();
            var vectorToScreenCenter = screenCenter - pointInScreenSpace;
            var vectorResizedToDiagramSpace = vectorToScreenCenter / Zoom;
            var projectedPoint = CenterInDiagramSpace - vectorResizedToDiagramSpace;
            return projectedPoint;
        }

        private Rect ProjectViewportIntoDiagramSpace()
        {
            if (SizeInScreenSpace.IsEmpty)
                return new Rect(Size.Empty);

            var projectedSize = new Size(SizeInScreenSpace.Width / Zoom, SizeInScreenSpace.Height / Zoom);
            var projectedTopLeft = new Point(CenterInDiagramSpace.X - projectedSize.Width / 2, CenterInDiagramSpace.Y - projectedSize.Height / 2);
            return new Rect(projectedTopLeft, projectedSize);
        }

        private Transform CalculateTransformForDiagramSpaceToScreenSpace(Rect viewportInDiagramSpace)
        {
            var translateVector = (Vector)viewportInDiagramSpace.TopLeft * -1;

            var transform = new TransformGroup();
            transform.Children.Add(new TranslateTransform(translateVector.X, translateVector.Y));
            transform.Children.Add(new ScaleTransform(Zoom, Zoom));
            return transform;
        }

        private void AddMiniButtons(Control control)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(control);

            var closeMiniButton = CreateCloseMiniButton(control);
            AddMiniButton(adornerLayer, closeMiniButton);

            if (DiagramBehaviourProvider != null)
            {
                foreach (var descriptor in DiagramBehaviourProvider.GetRelatedEntityMiniButtonDescriptors())
                {
                    var relatedEntityMiniButton = CreateRelatedEntityMiniButton(control, descriptor);
                    AddMiniButton(adornerLayer, relatedEntityMiniButton);
                }
            }
        }

        private void AddMiniButton(AdornerLayer adornerLayer, MiniButtonBase miniButton)
        {
            adornerLayer.Add(miniButton);
            _miniButtons.Add(miniButton);
        }

        private void RemoveMiniButtons(Control control)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(control);

            foreach (var miniButton in _miniButtons)
            {
                miniButton.MouseEnter -= OnMiniButtonMouseEnterOrLeave;
                miniButton.MouseLeave -= OnMiniButtonMouseEnterOrLeave;
                adornerLayer.Remove(miniButton);
            }

            _miniButtons.Clear();
        }

        private CloseMiniButton CreateCloseMiniButton(Control control)
        {
            var closeMiniButton = new CloseMiniButton(control);
            closeMiniButton.MouseEnter += OnMiniButtonMouseEnterOrLeave;
            closeMiniButton.MouseLeave += OnMiniButtonMouseEnterOrLeave;
            closeMiniButton.PreviewMouseLeftButtonDown += OnCloseButtonClick;
            return closeMiniButton;
        }

        private ShowRelatedEntityMiniButton CreateRelatedEntityMiniButton(Control control, RelatedEntityMiniButtonDescriptor relatedEntityMiniButtonDescriptor)
        {
            var showRelatedEntityMiniButton = new ShowRelatedEntityMiniButton(relatedEntityMiniButtonDescriptor, control);
            showRelatedEntityMiniButton.MouseEnter += OnMiniButtonMouseEnterOrLeave;
            showRelatedEntityMiniButton.MouseLeave += OnMiniButtonMouseEnterOrLeave;
            //miniButton.PreviewMouseLeftButtonDown += OnRelatedEntityMiniButtonClick;
            return showRelatedEntityMiniButton;
        }

        private void OnDiagramNodeLeftButtonDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            var senderDiagramNode = sender as DiagramNodeControl;
            if (senderDiagramNode == null || !ControlToDiagramShapeMap.Contains(senderDiagramNode))
                return;

            Diagram.SelectShape(ControlToDiagramShapeMap.Get(senderDiagramNode));
        }

        private void OnDiagramNodeDoubleClicked(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            var diagramNode = sender as DiagramNodeControl;
            if (diagramNode == null || !ControlToDiagramShapeMap.Contains(diagramNode))
                return;

            Diagram.ActivateShape(ControlToDiagramShapeMap.Get(diagramNode));
            mouseButtonEventArgs.Handled = true;
        }

        private void OnCloseButtonClick(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            var diagramNode = ((CloseMiniButton)sender).AdornedElement as DiagramNodeControl;
            if (diagramNode == null || !ControlToDiagramShapeMap.Contains(diagramNode))
                return;

            Diagram.RemoveShape(ControlToDiagramShapeMap.Get(diagramNode));
            mouseButtonEventArgs.Handled = true;
        }

        private void OnDiagramNodeMouseEnterOrLeave(object sender, MouseEventArgs mouseEventArgs)
        {
            HitTestAndShowOrHideMiniButton((DiagramShapeControlBase)sender, mouseEventArgs);
        }

        private void OnMiniButtonMouseEnterOrLeave(object sender, MouseEventArgs mouseEventArgs)
        {
            var miniButton = (MiniButtonBase)sender;
            HitTestAndShowOrHideMiniButton((DiagramShapeControlBase)miniButton.AdornedElement, mouseEventArgs);
        }

        private void HitTestAndShowOrHideMiniButton(DiagramShapeControlBase control, MouseEventArgs mouseEventArgs)
        {
            var hitItem = DiagramHitTester.HitTest(mouseEventArgs);

            if (hitItem == control || _miniButtons.Contains(hitItem))
                ShowMiniButtons(control);
            else
                HideMiniButtons(control);
        }

        private void ShowMiniButtons(Control control)
        {
            if (!_miniButtons.Any())
                AddMiniButtons(control);
        }

        private void HideMiniButtons(Control control)
        {
            if (_miniButtons.Any())
                RemoveMiniButtons(control);
        }
    }
}