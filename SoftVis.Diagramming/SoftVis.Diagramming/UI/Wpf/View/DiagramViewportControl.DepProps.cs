using System.Windows.Input;
using System.Windows.Media;

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

        public ICommand WidgetPanCommand
        {
            get { return (ICommand)GetValue(WidgetPanCommandProperty); }
            set { SetValue(WidgetPanCommandProperty, value); }
        }

        public ICommand WidgetZoomCommand
        {
            get { return (ICommand)GetValue(WidgetZoomCommandProperty); }
            set { SetValue(WidgetZoomCommandProperty, value); }
        }

        public ICommand WidgetZoomToContentCommand
        {
            get { return (ICommand)GetValue(WidgetZoomToContentCommandProperty); }
            set { SetValue(WidgetZoomToContentCommandProperty, value); }
        }

        public ICommand MousePanCommand
        {
            get { return (ICommand)GetValue(MousePanCommandProperty); }
            set { SetValue(MousePanCommandProperty, value); }
        }

        public ICommand MouseZoomCommand
        {
            get { return (ICommand)GetValue(MouseZoomCommandProperty); }
            set { SetValue(MouseZoomCommandProperty, value); }
        }

        public ICommand KeyboardPanCommand
        {
            get { return (ICommand)GetValue(KeyboardPanCommandProperty); }
            set { SetValue(KeyboardPanCommandProperty, value); }
        }

        public ICommand KeyboardZoomCommand
        {
            get { return (ICommand)GetValue(KeyboardZoomCommandProperty); }
            set { SetValue(KeyboardZoomCommandProperty, value); }
        }

        public ICommand ViewportResizeCommand
        {
            get { return (ICommand)GetValue(ViewportResizeCommandProperty); }
            set { SetValue(ViewportResizeCommandProperty, value); }
        }

        public ICommand ViewportPanCommand
        {
            get { return (ICommand)GetValue(ViewportPanCommandProperty); }
            set { SetValue(ViewportPanCommandProperty, value); }
        }

        public ICommand ViewportZoomCommand
        {
            get { return (ICommand)GetValue(ViewportZoomCommandProperty); }
            set { SetValue(ViewportZoomCommandProperty, value); }
        }

        public ICommand ViewportZoomToContentCommand
        {
            get { return (ICommand)GetValue(ViewportZoomToContentCommandProperty); }
            set { SetValue(ViewportZoomToContentCommandProperty, value); }
        }
    }
}
