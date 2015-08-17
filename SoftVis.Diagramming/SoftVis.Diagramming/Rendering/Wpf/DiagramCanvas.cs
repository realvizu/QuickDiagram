using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Rendering.Common;
using Codartis.SoftVis.Rendering.Common.UIEvents;
using Codartis.SoftVis.Rendering.Wpf.Common;
using Codartis.SoftVis.Rendering.Wpf.DiagramFixtures;
using Codartis.SoftVis.Rendering.Wpf.InputControls;
using Codartis.SoftVis.Rendering.Wpf.ViewportHandling;
using Codartis.SoftVis.Rendering.Wpf.ViewportHandling.Commands;
using Codartis.SoftVis.Rendering.Wpf.ViewportHandling.Gestures;

namespace Codartis.SoftVis.Rendering.Wpf
{
    /// <summary>
    /// Creates the visual representation of a Diagram.
    /// Manages and arranges controls created for diagram shapes.
    /// Viewport handling, transform calculation and user input handling are factored out to separate classes.
    /// </summary>
    [TemplatePart(Name = PART_Canvas, Type = typeof(Canvas))]
    [TemplatePart(Name = PART_PanAndZoomControl, Type = typeof(PanAndZoomControl))]
    public partial class DiagramCanvas : Control, IViewportHost, IUIEventSource
    {
        private const string PART_Canvas = "PART_Canvas";
        private const string PART_PanAndZoomControl = "PART_PanAndZoomControl";

        private readonly Viewport _viewport = new Viewport();
        private readonly List<IViewportGesture> _gestures = new List<IViewportGesture>();

        private readonly Dictionary<DiagramNode, DiagramNodeControl> _diagramNodeControls = new Dictionary<DiagramNode, DiagramNodeControl>();
        private readonly Dictionary<DiagramConnector, DiagramConnectorControl> _diagramConnectorControls = new Dictionary<DiagramConnector, DiagramConnectorControl>();
        private Rect _contentInDiagramSpace;

        private Canvas _canvas;
        private PanAndZoomControl _panAndZoomControl;

        static DiagramCanvas()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DiagramCanvas), new FrameworkPropertyMetadata(typeof(DiagramCanvas)));
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

        public double ViewportZoom
        {
            get { return _viewport.Zoom; }
        }

        public Rect ViewportInScreenSpace
        {
            get { return _viewport.ViewportInScreenSpace; }
        }

        public Rect ViewportInDiagramSpace
        {
            get { return _viewport.ViewportInDiagramSpace; }
        }

        public Rect ContentInDiagramSpace
        {
            get { return _contentInDiagramSpace; }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Focusable = true;
            Focus();

            InitializeGestures();

            _canvas = GetTemplateChild(PART_Canvas) as Canvas;
            _panAndZoomControl = GetTemplateChild(PART_PanAndZoomControl) as PanAndZoomControl;
            _panAndZoomControl.Pan += Pan;
            _panAndZoomControl.FitToView += FitToView;
            _panAndZoomControl.Zoom += PropagateZoomEvent;
            _panAndZoomControl.ZoomValue = _viewport.Zoom;
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            SendResizeEvent(sizeInfo.NewSize);

            FitToView(this, EventArgs.Empty);
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            base.ArrangeOverride(arrangeSize);

            if (_canvas != null)
            {
                foreach (var child in _canvas.Children.OfType<DiagramNodeControl>())
                    ArrangeChildControl(child, child.DiagramNode.Rect);

                foreach (var child in _canvas.Children.OfType<DiagramConnectorControl>())
                    ArrangeChildControl(child, child.DiagramConnector.Rect);
            }

            return arrangeSize;
        }

        private void InitializeGestures()
        {
            _gestures.Add(new AnimatedViewportGesture(new FitToViewUIEventViewportGesture(this), TimeSpan.FromMilliseconds(500)));
            _gestures.Add(new AnimatedViewportGesture(new ZoomUIEventViewportGesture(this), TimeSpan.FromMilliseconds(200)));
            _gestures.Add(new AnimatedViewportGesture(new PanUIEventViewportGesture(this), TimeSpan.FromMilliseconds(200)));
            _gestures.Add(new AnimatedViewportGesture(new MouseZoomViewportGesture(this), TimeSpan.FromMilliseconds(200)));
            _gestures.Add(new MousePanViewportGesture(this));
            _gestures.Add(new KeyboardViewportGesture(this));
            _gestures.Add(new ResizeViewportGesture(this));

            foreach (var gesture in _gestures)
            {
                gesture.ViewportCommand += OnViewportCommand;
            }
        }

        private void PropagateZoomEvent(object sender, ZoomEventArgs args)
        {
            if (_viewport.Zoom.EqualsWithTolerance(args.NewZoomValue))
                return;

            if (Zoom != null)
            {
                Zoom(sender, args);
            }
        }

        private void SendResizeEvent(Size newSize)
        {
            if (Resize != null)
                Resize(this, new ResizeEventArgs(newSize.Width, newSize.Height));
        }

        private void OnViewportCommand(object sender, ViewportCommandBase command)
        {
            command.Execute(_viewport);
            InvalidateVisual();

            if (!_viewport.Zoom.EqualsWithTolerance(_panAndZoomControl.ZoomValue))
                _panAndZoomControl.ZoomValue = _viewport.Zoom;
        }

        private void ArrangeChildControl(UIElement child, DiagramRect childRect)
        {
            var rect = childRect.ToWpf();
            child.Arrange(new Rect(rect.Size));
            child.RenderTransform = CreateTransformForChild(rect.Left, rect.Top);
        }

        private Transform CreateTransformForChild(double positionX, double positionY)
        {
            var transform = new TransformGroup();
            transform.Children.Add(new TranslateTransform(positionX, positionY));
            transform.Children.Add(_viewport.DiagramSpaceToScreenSpace);
            return transform;
        }

        private static void Diagram_PropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var diagramCanvas = (DiagramCanvas)obj;
            var diagram = (Diagram)e.NewValue;
            diagramCanvas.AddDiagram(diagram);
        }

        private void AddDiagram(Diagram diagram)
        {
            AddAllGraphElements(diagram);

            _contentInDiagramSpace = diagram.GetEnclosingRect().ToWpf();

            FitToView(this, EventArgs.Empty);
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
            _diagramNodeControls.Add(diagramNode, control);
            _canvas.Children.Insert(0, control);
        }

        private void CreateDiagramConnectorControl(DiagramConnector diagramConnector)
        {
            var control = DiagramConnectorControlFactory.CreateFrom(diagramConnector, _diagramNodeControls);
            _diagramConnectorControls.Add(diagramConnector, control);
            _canvas.Children.Add(control);
        }
    }
}
