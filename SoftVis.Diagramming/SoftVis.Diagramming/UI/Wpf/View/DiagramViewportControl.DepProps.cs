using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Codartis.SoftVis.UI.Wpf.Animations;

namespace Codartis.SoftVis.UI.Wpf.View
{
    partial class DiagramViewportControl
    {
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

        public double InitialZoom
        {
            get { return (double)GetValue(InitialZoomProperty); }
            set { SetValue(InitialZoomProperty, value); }
        }

        public double LargeZoomIncrement
        {
            get { return (double)GetValue(LargeZoomIncrementProperty); }
            set { SetValue(LargeZoomIncrementProperty, value); }
        }

        public double PanAndZoomControlHeight
        {
            get { return (double)GetValue(PanAndZoomControlHeightProperty); }
            set { SetValue(PanAndZoomControlHeightProperty, value); }
        }

        public double LinearViewportZoom
        {
            get { return (double)GetValue(LinearViewportZoomProperty); }
            set { SetValue(LinearViewportZoomProperty, value); }
        }

        public double ViewportCenterX
        {
            get { return (double)GetValue(ViewportCenterXProperty); }
            set { SetValue(ViewportCenterXProperty, value); }
        }

        public double ViewportCenterY
        {
            get { return (double)GetValue(ViewportCenterYProperty); }
            set { SetValue(ViewportCenterYProperty, value); }
        }

        public Transform ViewportTransform
        {
            get { return (Transform)GetValue(ViewportTransformProperty); }
            set { SetValue(ViewportTransformProperty, value); }
        }

        public AnimatedTransform AnimatedViewportTransform
        {
            get { return (AnimatedTransform)GetValue(AnimatedViewportTransformProperty); }
            set { SetValue(AnimatedViewportTransformProperty, value); }
        }

        public Rect DiagramContentRect
        {
            get { return (Rect)GetValue(DiagramContentRectProperty); }
            set { SetValue(DiagramContentRectProperty, value); }
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

        public ICommand FitToContentCommand
        {
            get { return (ICommand)GetValue(FitToContentCommandProperty); }
            set { SetValue(FitToContentCommandProperty, value); }
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

    }
}
