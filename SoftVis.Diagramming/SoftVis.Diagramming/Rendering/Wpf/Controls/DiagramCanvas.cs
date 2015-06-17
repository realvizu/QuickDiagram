using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Gesture handling is factored out to different classes.
    /// </summary>
    public class DiagramCanvas : Panel, IGestureTarget, IPanAndZoomEventSource
    {
        private const double _minScale = 0.3d;
        private const double _maxScale = 3d;
        private const double _exponentialScaleBase = 10d;

        private readonly Dictionary<DiagramNode, DiagramNodeControl> _diagramNodeControls = new Dictionary<DiagramNode, DiagramNodeControl>();
        private readonly List<IGesture> _gestures = new List<IGesture>();
        private PanAndZoomControl _panAndZoomControl;
        private double _linearScale;

        public double ExponentialScale { get; private set; }
        public Vector Translate { get; private set; }

        public event PanEventHandler Pan;
        public event ZoomEventHandler Zoom;

        static DiagramCanvas()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DiagramCanvas), new FrameworkPropertyMetadata(typeof(DiagramCanvas)));
        }

        public DiagramCanvas()
        {
            LinearScale = 1.0;
            Translate = new Vector(0, 0);

            InitializeGestures();
            InitializePanAndZoomControl();

            Focusable = true;
            Focus();
        }

        public double MinScale { get { return _minScale; } }
        public double MaxScale { get { return _maxScale; } }

        public double LinearScale
        {
            get { return _linearScale; }
            private set
            {
                _linearScale = value;
                ExponentialScale = LinearToExponentialScale(_linearScale);
            }
        }

        private void InitializePanAndZoomControl()
        {
            _panAndZoomControl = new PanAndZoomControl(_minScale, _maxScale, 1);
            _panAndZoomControl.Zoom += Zoom;
            Children.Add(_panAndZoomControl);
        }

        #region Diagram property

        public static readonly DependencyProperty DiagramProperty =
            DependencyProperty.Register("Diagram", typeof(Diagram), typeof(DiagramCanvas),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, Diagram_PropertyChanged));

        public Diagram Diagram
        {
            get { return (Diagram)GetValue(DiagramProperty); }
            set { SetValue(DiagramProperty, value); }
        }

        private static void Diagram_PropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var diagramCanvas = (DiagramCanvas)obj;
            diagramCanvas.AddAllGraphElements();
        }

        #endregion

        #region Gestures

        private void InitializeGestures()
        {
            _gestures.Add(new AnimatedGesture(new MouseZoomGesture(this), TimeSpan.FromMilliseconds(200)));
            //_gestures.Add(new MouseZoomGesture(this));
            _gestures.Add(new ControlZoomGesture(this));
            _gestures.Add(new MousePanGesture(this));
            _gestures.Add(new KeyboardPanAndZoomGesture(this));

            foreach (var gesture in _gestures)
            {
                gesture.ScaleChanged += OnScaleChanged;
                gesture.TranslateChanged += OnTranslateChanged;
            }
        }

        private void OnScaleChanged(object sender, ScaleChangedEventArgs args)
        {
            var oldScale = ExponentialScale;
            LinearScale = args.NewScale;
            Translate = (Translate + (Vector)args.ScaleCenter) * (ExponentialScale / oldScale) - (Vector)args.ScaleCenter;
            Debug.WriteLine("ScaleChanged: T={0} Old={1} New={2}", Translate, oldScale, ExponentialScale);
            InvalidateVisual();

            _panAndZoomControl.ZoomValue = LinearScale;
        }

        private void OnTranslateChanged(object sender, TranslateChangedEventArgs args)
        {
            Translate = new Vector(args.NewTranslate.X, args.NewTranslate.Y);
            InvalidateVisual();
        }

        private static double LinearToExponentialScale(double linearScale)
        {
            var linearScaleFromZeroToOne = (linearScale - _minScale) / (_maxScale - _minScale);
            var exponentialScaleFromZeroToOne = (Math.Pow(_exponentialScaleBase, linearScaleFromZeroToOne) - 1) / (_exponentialScaleBase - 1);
            return exponentialScaleFromZeroToOne * (_maxScale - _minScale) + _minScale;
        }
        #endregion

        #region Arrange

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            //Debug.WriteLine("ArrangeOverride {0}", arrangeSize);

            foreach (UIElement child in InternalChildren)
            {
                var diagramNodeControl = child as DiagramNodeControl;
                if (diagramNodeControl != null)
                {
                    ArrangeChildControl(child, diagramNodeControl.DiagramNode.Position, diagramNodeControl.DiagramNode.Size);
                }
            }

            _panAndZoomControl.Arrange(new Rect(10, 10, 100, 100));

            return arrangeSize;
        }

        private void ArrangeChildControl(UIElement child, DiagramPoint diagramPosition, DiagramSize diagramSize)
        {
            var x = diagramPosition.X * ExponentialScale - Translate.X;
            var y = diagramPosition.Y * ExponentialScale - Translate.Y;

            child.Arrange(new Rect(x, y, diagramSize.Width, diagramSize.Height));

            SetChildRenderTransform(child);
        }

        private void SetChildRenderTransform(UIElement child)
        {
            var scaleTransform = child.RenderTransform as ScaleTransform;
            if (scaleTransform != null)
            {
                scaleTransform.ScaleX = ExponentialScale;
                scaleTransform.ScaleY = ExponentialScale;
            }
            else
            {
                child.RenderTransform = new ScaleTransform(ExponentialScale, ExponentialScale);
            }
        }

        #endregion

        private void AddAllGraphElements()
        {
            foreach (var vertex in Diagram.Nodes)
                CreateDiagramNodeControl(vertex);
        }

        private void CreateDiagramNodeControl(DiagramNode diagramNode)
        {
            var control = DiagramNodeControlFactory.CreateFrom(diagramNode);
            _diagramNodeControls.Add(diagramNode, control);
            Children.Insert(0, control);
        }
    }
}
