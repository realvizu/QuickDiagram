using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Rendering.Wpf.Common;
using Codartis.SoftVis.Rendering.Wpf.DiagramRendering.ShapeControls;
using Codartis.SoftVis.Rendering.Wpf.DiagramRendering.ShapeControls.Adorners;

namespace Codartis.SoftVis.Rendering.Wpf.DiagramRendering.Viewport
{
    /// <summary>
    /// Renders the visible part of a Diagram.
    /// </summary>
    internal class DiagramViewportPanel : DiagramPanelBase, IDiagramViewport
    {
        private readonly Dictionary<Control, List<Adorner>> _controlToAdornersMap;

        private double _zoom;
        private Size _sizeInScreenSpace;
        private Point _centerInDiagramSpace;

        public Rect ViewportInScreenSpace { get; private set; }
        public Rect ViewportInDiagramSpace { get; private set; }
        private Transform DiagramSpaceToScreenSpace { get; set; }

        public static readonly DependencyProperty MinZoomProperty =
            DependencyProperty.Register("MinZoom", typeof(double), typeof(DiagramViewportPanel));

        public static readonly DependencyProperty MaxZoomProperty =
            DependencyProperty.Register("MaxZoom", typeof(double), typeof(DiagramViewportPanel));

        public DiagramViewportPanel()
        {
            _controlToAdornersMap = new Dictionary<Control, List<Adorner>>();

            _zoom = 1.0;
            _sizeInScreenSpace = Size.Empty;
            _centerInDiagramSpace = new Point(0, 0);

            RecalculateCachedValues();
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

            AddAdorners(control);

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

        private void AddAdorners(Control control)
        {
            var closeButtonAdorner = new CloseButtonAdorner(control);
            closeButtonAdorner.MouseEnter += OnCloseButtonAdornerMouseEnterOrLeave;
            closeButtonAdorner.MouseLeave += OnCloseButtonAdornerMouseEnterOrLeave;
            closeButtonAdorner.PreviewMouseLeftButtonDown += OnCloseButtonClick;

            var adornerLayer = AdornerLayer.GetAdornerLayer(control);
            if (adornerLayer == null)
                throw new Exception("No adorner layer.");
            adornerLayer.Add(closeButtonAdorner);

            _controlToAdornersMap[control] = new List<Adorner> { closeButtonAdorner };
        }

        private void OnDiagramNodeLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var senderDiagramNode = sender as DiagramNodeControl;
            if (senderDiagramNode == null || !ControlToDiagramShapeMap.Contains(senderDiagramNode))
                return;

            Diagram.SelectShape(ControlToDiagramShapeMap.Get(senderDiagramNode));
        }

        private void OnDiagramNodeDoubleClicked(object sender, MouseButtonEventArgs e)
        {
            var diagramNode = sender as DiagramNodeControl;
            if (diagramNode == null || !ControlToDiagramShapeMap.Contains(diagramNode))
                return;

            Diagram.ActivateShape(ControlToDiagramShapeMap.Get(diagramNode));
            e.Handled = true;
        }

        private void OnCloseButtonClick(object sender, MouseButtonEventArgs e)
        {
            var diagramNode = ((CloseButtonAdorner)sender).AdornedElement as DiagramNodeControl;
            if (diagramNode == null || !ControlToDiagramShapeMap.Contains(diagramNode))
                return;

            Diagram.RemoveShape(ControlToDiagramShapeMap.Get(diagramNode));
            e.Handled = true;
        }

        private void OnDiagramNodeMouseEnterOrLeave(object sender, MouseEventArgs e)
        {
            HitTestAndSetAdornersVisibility((DiagramShapeControlBase)sender, e);
        }

        private void OnCloseButtonAdornerMouseEnterOrLeave(object sender, MouseEventArgs e)
        {
            var adorner = (Adorner)sender;
            HitTestAndSetAdornersVisibility((DiagramShapeControlBase)adorner.AdornedElement, e);
        }

        private void HitTestAndSetAdornersVisibility(DiagramShapeControlBase control, MouseEventArgs e)
        {
            var adorners = _controlToAdornersMap[control];
            var hitTestSubjects = new List<UIElement> { control }.Concat(adorners);
            var hit = hitTestSubjects.Any(i => VisualTreeHelper.HitTest(i, e.GetPosition(i)) != null);

            SetAdornersVisibility(adorners, hit ? Visibility.Visible : Visibility.Collapsed);
        }

        private static void SetAdornersVisibility(IEnumerable<Adorner> adorners, Visibility visibility)
        {
            if (adorners != null)
                foreach (var adorner in adorners)
                    adorner.Visibility = visibility;
        }
    }
}