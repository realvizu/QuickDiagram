using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Rendering.Common;
using Codartis.SoftVis.Rendering.Common.UIEvents;
using Codartis.SoftVis.Rendering.Wpf.Common;
using Codartis.SoftVis.Rendering.Wpf.DiagramFixtures;
using Codartis.SoftVis.Rendering.Wpf.InputControls;
using Codartis.SoftVis.Rendering.Wpf.Viewport;
using Codartis.SoftVis.Rendering.Wpf.Viewport.Commands;
using Codartis.SoftVis.Rendering.Wpf.Viewport.Gestures;
using Codartis.SoftVis.Rendering.Wpf.Viewport.Gestures.Animated;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Codartis.SoftVis.Rendering.Wpf
{
    /// <summary>
    /// Manages diagram shape to control conversions.
    /// Routes user input events to gestures that translate them to viewport commands.
    /// </summary>
    [TemplatePart(Name = PART_ViewportPanel, Type = typeof(ViewportPanel))]
    [TemplatePart(Name = PART_PanAndZoomControl, Type = typeof(PanAndZoomControl))]
    public partial class DiagramCanvasControl : Control, IInputElement, IWidgetEventSource
    {
        private const string PART_ViewportPanel = "PART_ViewportPanel";
        private const string PART_PanAndZoomControl = "PART_PanAndZoomControl";

        private ViewportPanel _viewportPanel;
        private PanAndZoomControl _panAndZoomControl;

        private readonly List<IViewportGesture> _gestures = new List<IViewportGesture>();
        private readonly Dictionary<DiagramShape, DiagramShapeControlBase> _diagramShapeControls = new Dictionary<DiagramShape, DiagramShapeControlBase>();

        static DiagramCanvasControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DiagramCanvasControl), new FrameworkPropertyMetadata(typeof(DiagramCanvasControl)));
        }

        public event PanEventHandler Pan;
        public event ZoomEventHandler Zoom;
        public event EventHandler FitToView;
        public event ResizeEventHandler Resize;

        public Diagram Diagram
        {
            get { return (Diagram)GetValue(DiagramProperty); }
            set { SetValue(DiagramProperty, value); }
        }

        public Rect DiagramRect
        {
            get { return (Rect)GetValue(DiagramRectProperty); }
            set { SetValue(DiagramRectProperty, value); }
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

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Focusable = true;
            Focus();

            _viewportPanel = GetTemplateChild(PART_ViewportPanel) as ViewportPanel;

            _panAndZoomControl = GetTemplateChild(PART_PanAndZoomControl) as PanAndZoomControl;
            _panAndZoomControl.Pan += OnPan;
            _panAndZoomControl.FitToView += OnFitToView;
            _panAndZoomControl.Zoom += OnZoom;
            _panAndZoomControl.ZoomValue = _viewportPanel.Zoom;

            InitializeGestures();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            OnResize(sizeInfo.NewSize);
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            base.ArrangeOverride(arrangeBounds);

            if (_viewportPanel != null)
                _viewportPanel.Arrange(new Rect(arrangeBounds));

            return arrangeBounds;
        }

        private void InitializeGestures()
        {
            _gestures.Add(new AnimatedViewportGesture(new FitToViewWidgetEventViewportGesture(_viewportPanel, this), TimeSpan.FromMilliseconds(500)));
            _gestures.Add(new AnimatedViewportGesture(new ZoomWidgetEventViewportGesture(_viewportPanel, this), TimeSpan.FromMilliseconds(200)));
            _gestures.Add(new AnimatedViewportGesture(new PanWidgetEventViewportGesture(_viewportPanel, this), TimeSpan.FromMilliseconds(200)));
            _gestures.Add(new AnimatedViewportGesture(new MouseZoomViewportGesture(_viewportPanel, this), TimeSpan.FromMilliseconds(200)));
            _gestures.Add(new MousePanViewportGesture(_viewportPanel, this));
            _gestures.Add(new KeyboardViewportGesture(_viewportPanel, this));
            _gestures.Add(new ResizeViewportGesture(_viewportPanel, this));

            foreach (var gesture in _gestures)
            {
                gesture.ViewportCommand += OnViewportCommand;
            }
        }

        private void OnPan(object sender, PanEventArgs args)
        {
            if (Pan != null)
                Pan(sender, args);
        }

        private void OnFitToView(object sender = null, EventArgs args = null)
        {
            if (FitToView != null)
                FitToView(sender ?? this, args ?? EventArgs.Empty);
        }

        private void OnZoom(object sender, ZoomEventArgs args)
        {
            if (_viewportPanel.Zoom.EqualsWithTolerance(args.NewZoomValue))
                return;

            if (Zoom != null)
                Zoom(sender, args);
        }

        private void OnResize(Size newSize)
        {
            if (Resize != null)
                Resize(this, new ResizeEventArgs(newSize.Width, newSize.Height));
        }

        private void OnViewportCommand(object sender, ViewportCommandBase command)
        {
            command.Execute(_viewportPanel);
            _viewportPanel.InvalidateArrange();

            if (ViewportZoomMatchesWidgetZoom())
                return;

            if (IsCommandAnimatedFromZoomWidgetEvent(command))
                return;

            _panAndZoomControl.ZoomValue = _viewportPanel.Zoom;
        }

        private bool ViewportZoomMatchesWidgetZoom()
        {
            return _viewportPanel.Zoom.EqualsWithTolerance(_panAndZoomControl.ZoomValue);
        }

        private bool IsCommandAnimatedFromZoomWidgetEvent(ViewportCommandBase command)
        {
            return command.Sender is ZoomWidgetEventViewportGesture;
        }

        private static void Diagram_PropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var diagramCanvas = (DiagramCanvasControl)obj;
            var diagram = (Diagram)e.NewValue;
            diagramCanvas.AddDiagram(diagram);

            diagram.ShapeAdded += diagramCanvas.OnShapeAdded;
            diagram.ShapeModified += diagramCanvas.OnShapeModified;
            diagram.ShapeRemoved += diagramCanvas.OnShapeRemoved;
            diagram.Cleared += diagramCanvas.OnDiagramCleared;
        }

        private void AddDiagram(Diagram diagram)
        {
            AddAllGraphElements(diagram);
            OnDiagramChanged();
        }

        private void OnShapeAdded(object sender, DiagramShape shape)
        {
            if (_diagramShapeControls.ContainsKey(shape))
                return;

            if (shape is DiagramNode)
                CreateDiagramNodeControl((DiagramNode)shape);
            else if (shape is DiagramConnector)
                CreateDiagramConnectorControl((DiagramConnector)shape);
            OnDiagramChanged();
        }

        private void OnShapeModified(object sender, DiagramShape shape)
        {
            if (!_diagramShapeControls.ContainsKey(shape))
                return;

            _diagramShapeControls[shape].Update();
            OnDiagramChanged();
        }

        private void OnShapeRemoved(object sender, DiagramShape shape)
        {
            if (!_diagramShapeControls.ContainsKey(shape))
                return;

            RemoveDiagramShapeControl(shape);
            OnDiagramChanged();
        }

        private void OnDiagramCleared(object sender, EventArgs e)
        {
            _viewportPanel.Children.Clear();
            _diagramShapeControls.Clear();
            OnDiagramChanged();
        }

        private void OnDiagramChanged()
        {
            DiagramRect = Diagram.GetEnclosingRect().ToWpf();
            OnFitToView();
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
            var control = DiagramNodeControlFactory.CreateFrom(diagramNode);
            _diagramShapeControls.Add(diagramNode, control);
            _viewportPanel.Children.Add(control);
        }

        private void CreateDiagramConnectorControl(DiagramConnector diagramConnector)
        {
            var control = DiagramConnectorControlFactory.CreateFrom(diagramConnector, _diagramShapeControls);
            _diagramShapeControls.Add(diagramConnector, control);
            _viewportPanel.Children.Add(control);
        }

        private void RemoveDiagramShapeControl(DiagramShape diagramShape)
        {
            _viewportPanel.Children.Remove(_diagramShapeControls[diagramShape]);
            _diagramShapeControls.Remove(diagramShape);
        }
    }
}
