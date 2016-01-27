using System.Windows.Media;
using Codartis.SoftVis.UI.Wpf.Commands;

namespace Codartis.SoftVis.UI.Wpf.View
{
    partial class DiagramViewportControl
    {
        public double PanAndZoomControlHeight
        {
            get { return (double)GetValue(PanAndZoomControlHeightProperty); }
            set { SetValue(PanAndZoomControlHeightProperty, value); }
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

        public double LargeZoomIncrement
        {
            get { return (double)GetValue(LargeZoomIncrementProperty); }
            set { SetValue(LargeZoomIncrementProperty, value); }
        }

        public double ViewportZoom
        {
            get { return (double)GetValue(ViewportZoomProperty); }
            set { SetValue(ViewportZoomProperty, value); }
        }

        public Transform ViewportTransform
        {
            get { return (Transform)GetValue(ViewportTransformProperty); }
            set { SetValue(ViewportTransformProperty, value); }
        }

        public TransitionedTransform TransitionedViewportTransform
        {
            get { return (TransitionedTransform)GetValue(TransitionedViewportTransformProperty); }
            set { SetValue(TransitionedViewportTransformProperty, value); }
        }

        public VectorCommand WidgetPanCommand
        {
            get { return (VectorCommand)GetValue(WidgetPanCommandProperty); }
            set { SetValue(WidgetPanCommandProperty, value); }
        }

        public DoubleCommand WidgetZoomCommand
        {
            get { return (DoubleCommand)GetValue(WidgetZoomCommandProperty); }
            set { SetValue(WidgetZoomCommandProperty, value); }
        }

        public ParameterlessCommand WidgetZoomToContentCommand
        {
            get { return (ParameterlessCommand)GetValue(WidgetZoomToContentCommandProperty); }
            set { SetValue(WidgetZoomToContentCommandProperty, value); }
        }

        public VectorCommand MousePanCommand
        {
            get { return (VectorCommand)GetValue(MousePanCommandProperty); }
            set { SetValue(MousePanCommandProperty, value); }
        }

        public ZoomCommand MouseZoomCommand
        {
            get { return (ZoomCommand)GetValue(MouseZoomCommandProperty); }
            set { SetValue(MouseZoomCommandProperty, value); }
        }

        public VectorCommand KeyboardPanCommand
        {
            get { return (VectorCommand)GetValue(KeyboardPanCommandProperty); }
            set { SetValue(KeyboardPanCommandProperty, value); }
        }

        public ZoomCommand KeyboardZoomCommand
        {
            get { return (ZoomCommand)GetValue(KeyboardZoomCommandProperty); }
            set { SetValue(KeyboardZoomCommandProperty, value); }
        }

        public ViewportResizeCommand ViewportResizeCommand
        {
            get { return (ViewportResizeCommand)GetValue(ViewportResizeCommandProperty); }
            set { SetValue(ViewportResizeCommandProperty, value); }
        }

        public ViewportPanCommand ViewportPanCommand
        {
            get { return (ViewportPanCommand)GetValue(ViewportPanCommandProperty); }
            set { SetValue(ViewportPanCommandProperty, value); }
        }

        public ViewportZoomCommand ViewportZoomCommand
        {
            get { return (ViewportZoomCommand)GetValue(ViewportZoomCommandProperty); }
            set { SetValue(ViewportZoomCommandProperty, value); }
        }

        public ViewportZoomToContentCommand ViewportZoomToContentCommand
        {
            get { return (ViewportZoomToContentCommand)GetValue(ViewportZoomToContentCommandProperty); }
            set { SetValue(ViewportZoomToContentCommandProperty, value); }
        }
    }
}
