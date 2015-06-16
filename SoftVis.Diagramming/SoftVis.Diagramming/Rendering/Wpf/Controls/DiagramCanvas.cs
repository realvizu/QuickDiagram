using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
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
        private readonly Dictionary<DiagramNode, DiagramNodeControl> _diagramNodeControls = new Dictionary<DiagramNode, DiagramNodeControl>();
        private readonly PanAndZoomControl _panAndZoomControl = new PanAndZoomControl();
        private readonly List<IGesture> _gestures = new List<IGesture>();

        public double Scale { get; private set; }
        public Vector Translate { get; private set; }

        public event PanEventHandler Pan;
        public event ZoomEventHandler Zoom;

        static DiagramCanvas()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DiagramCanvas), new FrameworkPropertyMetadata(typeof(DiagramCanvas)));
        }

        public DiagramCanvas()
        {
            Scale = 1.0;
            Translate = new Vector(0, 0);

            InitializeGestures();
            InitializePanAndZoomControl();

            Focusable = true;
            Focus();
        }

        private void InitializePanAndZoomControl()
        {
            Children.Add(_panAndZoomControl);
            _panAndZoomControl.Zoom += Zoom;
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
            _gestures.Add(new AnimatedGesture(new ControlZoomGesture(this), TimeSpan.FromMilliseconds(200)));
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
            Scale = args.NewScale;
            InvalidateVisual();
        }

        private void OnTranslateChanged(object sender, TranslateChangedEventArgs args)
        {
            Translate = new Vector(args.NewTranslate.X, args.NewTranslate.Y);
            InvalidateVisual();
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
            var x = diagramPosition.X * Scale - Translate.X;
            var y = diagramPosition.Y * Scale - Translate.Y;

            child.Arrange(new Rect(x, y, diagramSize.Width, diagramSize.Height));

            SetChildRenderTransform(child);
        }

        private void SetChildRenderTransform(UIElement child)
        {
            var scaleTransform = child.RenderTransform as ScaleTransform;
            if (scaleTransform != null)
            {
                scaleTransform.ScaleX = Scale;
                scaleTransform.ScaleY = Scale;
            }
            else
            {
                child.RenderTransform = new ScaleTransform(Scale, Scale);
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
