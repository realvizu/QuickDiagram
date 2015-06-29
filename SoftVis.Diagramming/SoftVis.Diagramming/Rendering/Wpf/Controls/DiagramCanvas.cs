using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Rendering.Wpf.Gestures;

namespace Codartis.SoftVis.Rendering.Wpf.Controls
{
    /// <summary>
    /// Creates the visual representation of a Diagram.
    /// Manages and arranges controls created for diagram shapes.
    /// Gesture handling is factored out to separate classes.
    /// </summary>
    [TemplatePart(Name = PART_Canvas, Type = typeof(Canvas))]
    [TemplatePart(Name = PART_PanAndZoomControl, Type = typeof(PanAndZoomControl))]
    public partial class DiagramCanvas : Control, IGestureTarget, IPanAndZoomEventSource
    {
        private const string PART_Canvas = "PART_Canvas";
        private const string PART_PanAndZoomControl = "PART_PanAndZoomControl";
        private const double _exponentialScaleBase = 10d;

        private readonly Dictionary<DiagramNode, DiagramNodeControl> _diagramNodeControls = new Dictionary<DiagramNode, DiagramNodeControl>();
        private readonly List<IGesture> _gestures = new List<IGesture>();

        private Canvas _canvas;
        private PanAndZoomControl _panAndZoomControl;

        private double _linearScale;
        private double _exponentialScale;
        private Vector _translate;

        static DiagramCanvas()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DiagramCanvas), new FrameworkPropertyMetadata(typeof(DiagramCanvas)));
        }

        public event PanEventHandler Pan;
        public event ZoomEventHandler Zoom;

        public Diagram Diagram
        {
            get { return (Diagram)GetValue(DiagramProperty); }
            set { SetValue(DiagramProperty, value); }
        }

        public double MinScale
        {
            get { return (double)GetValue(MinScaleProperty); }
            set { SetValue(MinScaleProperty, value); }
        }

        public double MaxScale
        {
            get { return (double)GetValue(MaxScaleProperty); }
            set { SetValue(MaxScaleProperty, value); }
        }

        public double LinearScale
        {
            get { return _linearScale; }
        }

        public Vector Translate
        {
            get { return _translate; }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Focusable = true;
            Focus();

            _translate = new Vector(0, 0);
            SetScaleUsingExponentialValue(1.0);
            InitializeGestures();

            _canvas = GetTemplateChild(PART_Canvas) as Canvas;
            _panAndZoomControl = GetTemplateChild(PART_PanAndZoomControl) as PanAndZoomControl;
            _panAndZoomControl.Zoom += PropagateZoomEvent;
            _panAndZoomControl.ZoomValue = LinearScale;
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
                        ArrangeChildControl(child, diagramNodeControl.DiagramNode.Position, diagramNodeControl.DiagramNode.Size);
                    }
                }
            }

            return arrangeSize;
        }

        private void InitializeGestures()
        {
            _gestures.Add(new AnimatedGesture(new MouseZoomGesture(this), TimeSpan.FromMilliseconds(200)));
            _gestures.Add(new AnimatedGesture(new UIControlZoomGesture(this), TimeSpan.FromMilliseconds(200)));
            _gestures.Add(new MousePanGesture(this));
            _gestures.Add(new KeyboardPanAndZoomGesture(this));

            foreach (var gesture in _gestures)
            {
                gesture.ScaleChanged += OnScaleChanged;
                gesture.TranslateChanged += OnTranslateChanged;
            }
        }

        private void PropagateZoomEvent(object sender, ZoomEventArgs args)
        {
            if (Zoom != null && args.NewZoomValue != LinearScale)
            {
                Zoom(sender, args);
            }
        }

        private void OnScaleChanged(object sender, ScaleChangedEventArgs args)
        {
            if (args.NewScale == _linearScale)
                return;

            var oldScale = _exponentialScale;
            SetScaleUsingLinearValue(args.NewScale);
            _translate = (_translate + (Vector)args.ScaleCenter) * (_exponentialScale / oldScale) - (Vector)args.ScaleCenter;

            InvalidateVisual();

            if (!(sender is UIControlZoomGesture))
                _panAndZoomControl.ZoomValue = LinearScale;
        }

        private void OnTranslateChanged(object sender, TranslateChangedEventArgs args)
        {
            _translate = new Vector(args.NewTranslate.X, args.NewTranslate.Y);
            InvalidateVisual();
        }

        private void SetScaleUsingLinearValue(double linearScale)
        {
            _linearScale = linearScale;
            _exponentialScale = ScaleCalculator.LinearToExponential(linearScale, MinScale, MaxScale, _exponentialScaleBase);
        }

        private void SetScaleUsingExponentialValue(double exponentialScale)
        {
            _exponentialScale = exponentialScale;
            _linearScale = ScaleCalculator.ExponentialToLinear(exponentialScale, MinScale, MaxScale, _exponentialScaleBase);
        }

        private void ArrangeChildControl(UIElement child, DiagramPoint diagramPosition, DiagramSize diagramSize)
        {
            var x = diagramPosition.X * _exponentialScale - _translate.X;
            var y = diagramPosition.Y * _exponentialScale - _translate.Y;

            child.Arrange(new Rect(x, y, diagramSize.Width, diagramSize.Height));

            SetChildRenderTransform(child);
        }

        private void SetChildRenderTransform(UIElement child)
        {
            var scaleTransform = child.RenderTransform as ScaleTransform;
            if (scaleTransform != null)
            {
                scaleTransform.ScaleX = _exponentialScale;
                scaleTransform.ScaleY = _exponentialScale;
            }
            else
            {
                child.RenderTransform = new ScaleTransform(_exponentialScale, _exponentialScale);
            }
        }

        private static void Diagram_PropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var diagramCanvas = (DiagramCanvas)obj;
            diagramCanvas.AddAllGraphElements();
        }

        private void AddAllGraphElements()
        {
            foreach (var vertex in Diagram.Nodes)
                CreateDiagramNodeControl(vertex);
        }

        private void CreateDiagramNodeControl(DiagramNode diagramNode)
        {
            var control = DiagramNodeControlFactory.CreateFrom(diagramNode);
            _diagramNodeControls.Add(diagramNode, control);
            _canvas.Children.Insert(0, control);
        }
    }
}
