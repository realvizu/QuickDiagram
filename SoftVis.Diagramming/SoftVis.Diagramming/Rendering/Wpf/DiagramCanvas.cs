using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Rendering.Common;
using Codartis.SoftVis.Rendering.Common.UIEvents;
using Codartis.SoftVis.Rendering.Wpf.Common;
using Codartis.SoftVis.Rendering.Wpf.DiagramFixtures;
using Codartis.SoftVis.Rendering.Wpf.ViewportHandling;
using Codartis.SoftVis.Rendering.Wpf.ViewportHandling.Commands;
using Codartis.SoftVis.Rendering.Wpf.ViewportHandling.Gestures;

namespace Codartis.SoftVis.Rendering.Wpf
{
    /// <summary>
    /// Creates the visual representation of a Diagram.
    /// Manages and arranges controls created for diagram shapes.
    /// Gesture handling is factored out to separate classes.
    /// </summary>
    [TemplatePart(Name = PART_Canvas, Type = typeof(Canvas))]
    [TemplatePart(Name = PART_PanAndZoomControl, Type = typeof(PanAndZoomControl))]
    public partial class DiagramCanvas : Control, IViewportHost, IPanAndZoomEventSource
    {
        private const string PART_Canvas = "PART_Canvas";
        private const string PART_PanAndZoomControl = "PART_PanAndZoomControl";

        private readonly Viewport _viewport = new Viewport();
        private readonly List<IViewportGesture> _gestures = new List<IViewportGesture>();

        private readonly Dictionary<DiagramNode, DiagramNodeControl> _diagramNodeControls = new Dictionary<DiagramNode, DiagramNodeControl>();
        private Rect _contentRect;

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
            get { return _contentRect; }
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

            if (_viewport.SizeInScreenSpace.IsEmpty)
                _viewport.ResizeInScreenSpace(sizeInfo.NewSize);
            else
                SendResizeEvent(sizeInfo.NewSize);
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            base.ArrangeOverride(arrangeSize);

            if (_canvas != null)
            {
                foreach (UIElement child in _canvas.Children)
                {
                    var diagramNodeControl = child as DiagramNodeControl;
                    if (diagramNodeControl != null)
                    {
                        var childPosition = diagramNodeControl.DiagramNode.Position;
                        var childSize = diagramNodeControl.DiagramNode.Size;
                        ArrangeChildControl(child, childPosition, childSize);
                    }
                }
            }

            return arrangeSize;
        }

        private void InitializeGestures()
        {
            _gestures.Add(new AnimatedViewportGesture(new UIControlFitToViewportGesture(this), TimeSpan.FromMilliseconds(500)));
            _gestures.Add(new AnimatedViewportGesture(new UIControlZoomViewportGesture(this), TimeSpan.FromMilliseconds(200)));
            _gestures.Add(new AnimatedViewportGesture(new UIControlPanViewportGesture(this), TimeSpan.FromMilliseconds(200)));
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
            {
                Resize(this, new ResizeEventArgs(newSize.Width, newSize.Height));
            }
        }

        private void OnViewportCommand(object sender, ViewportCommandBase command)
        {
            command.Execute(_viewport);
            InvalidateVisual();

            if (!_viewport.Zoom.EqualsWithTolerance(_panAndZoomControl.ZoomValue))
                _panAndZoomControl.ZoomValue = _viewport.Zoom;
        }

        private void ArrangeChildControl(UIElement child, DiagramPoint childPosition, DiagramSize childSize)
        {
            child.Arrange(new Rect(0, 0, childSize.Width, childSize.Height));

            var transform = new TransformGroup();
            transform.Children.Add(new TranslateTransform(childPosition.X, childPosition.Y));
            transform.Children.Add(_viewport.DiagramSpaceToScreenSpace);
            child.RenderTransform = transform;
        }

        private static void Diagram_PropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var diagramCanvas = (DiagramCanvas)obj;
            var diagram = (Diagram)e.NewValue;

            diagramCanvas.AddAllGraphElements(diagram);
            diagramCanvas.SetContentRect(CalculateEnclosingRect(diagram.Nodes));
            diagramCanvas.FitToView(diagramCanvas, EventArgs.Empty);
        }

        private void SetContentRect(Rect contentRect)
        {
            _contentRect = contentRect;
        }

        private void AddAllGraphElements(Diagram diagram)
        {
            foreach (var vertex in diagram.Nodes)
                CreateDiagramNodeControl(vertex);

            var enclosingRect = CalculateEnclosingRect(Diagram.Nodes);
            var dummyClass = new ClassNode()
            {
                Position = new DiagramPoint(enclosingRect.X, enclosingRect.Y),
                Size = new DiagramSize(enclosingRect.Width, enclosingRect.Height),
                Name = "Enclosing",
            };
            CreateDiagramNodeControl(dummyClass);
        }

        private void CreateDiagramNodeControl(DiagramNode diagramNode)
        {
            var control = DiagramNodeControlFactory.CreateFrom(diagramNode);
            _diagramNodeControls.Add(diagramNode, control);
            _canvas.Children.Insert(0, control);
        }

        private static Rect CalculateEnclosingRect(IEnumerable<DiagramNode> nodes)
        {
            var enclosingRect = Rect.Empty;

            foreach (var node in nodes)
            {
                var childPosition = node.Position;
                var childSize = node.Size;
                enclosingRect = GetAdjustedEnclosingRect(enclosingRect, childPosition, childSize);
            }

            return enclosingRect;
        }

        private static Rect GetAdjustedEnclosingRect(Rect enclosingRect, DiagramPoint childPosition, DiagramSize childSize)
        {
            if (enclosingRect.IsEmpty)
                return new Rect(new Point(childPosition.X, childPosition.Y), new Size(childSize.Width, childSize.Height));

            if (childPosition.X < enclosingRect.X)
                enclosingRect.X = childPosition.X;

            if (childPosition.Y < enclosingRect.Y)
                enclosingRect.Y = childPosition.Y;

            if (childPosition.X + childSize.Width > enclosingRect.X + enclosingRect.Width)
                enclosingRect.Width = childPosition.X + childSize.Width - enclosingRect.X;

            if (childPosition.Y + childSize.Height > enclosingRect.Y + enclosingRect.Height)
                enclosingRect.Height = childPosition.Y + childSize.Height - enclosingRect.Y;

            return enclosingRect;
        }
    }
}
