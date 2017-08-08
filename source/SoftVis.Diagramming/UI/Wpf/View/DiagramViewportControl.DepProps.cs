using System.Windows;
using Codartis.SoftVis.UI.Wpf.Commands;
using Codartis.SoftVis.UI.Wpf.ViewModel;
using Codartis.SoftVis.Util.UI.Wpf.Commands;
using Codartis.SoftVis.Util.UI.Wpf.Transforms;

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

        public TransitionedTransform ViewportTransform
        {
            get { return (TransitionedTransform)GetValue(ViewportTransformProperty); }
            set { SetValue(ViewportTransformProperty, value); }
        }

        public DiagramNodeViewModelBase DecoratedDiagramNode
        {
            get { return (DiagramNodeViewModelBase)GetValue(DecoratedDiagramNodeProperty); }
            set { SetValue(DecoratedDiagramNodeProperty, value); }
        }

        public UIElement DecoratedDiagramNodeControl
        {
            get { return (UIElement)GetValue(DecoratedDiagramNodeControlProperty); }
            set { SetValue(DecoratedDiagramNodeControlProperty, value); }
        }

        public VectorDelegateCommand WidgetPanCommand
        {
            get { return (VectorDelegateCommand)GetValue(WidgetPanCommandProperty); }
            set { SetValue(WidgetPanCommandProperty, value); }
        }

        public DoubleDelegateCommand WidgetZoomCommand
        {
            get { return (DoubleDelegateCommand)GetValue(WidgetZoomCommandProperty); }
            set { SetValue(WidgetZoomCommandProperty, value); }
        }

        public DelegateCommand WidgetZoomToContentCommand
        {
            get { return (DelegateCommand)GetValue(WidgetZoomToContentCommandProperty); }
            set { SetValue(WidgetZoomToContentCommandProperty, value); }
        }

        public VectorDelegateCommand MousePanCommand
        {
            get { return (VectorDelegateCommand)GetValue(MousePanCommandProperty); }
            set { SetValue(MousePanCommandProperty, value); }
        }

        public ZoomDelegateCommand MouseZoomCommand
        {
            get { return (ZoomDelegateCommand)GetValue(MouseZoomCommandProperty); }
            set { SetValue(MouseZoomCommandProperty, value); }
        }

        public VectorDelegateCommand KeyboardPanCommand
        {
            get { return (VectorDelegateCommand)GetValue(KeyboardPanCommandProperty); }
            set { SetValue(KeyboardPanCommandProperty, value); }
        }

        public ZoomDelegateCommand KeyboardZoomCommand
        {
            get { return (ZoomDelegateCommand)GetValue(KeyboardZoomCommandProperty); }
            set { SetValue(KeyboardZoomCommandProperty, value); }
        }

        public ViewportCalculatorViewModel.ResizeDelegateCommand ViewportResizeCommand
        {
            get { return (ViewportCalculatorViewModel.ResizeDelegateCommand)GetValue(ViewportResizeCommandProperty); }
            set { SetValue(ViewportResizeCommandProperty, value); }
        }

        public ViewportCalculatorViewModel.PanDelegateCommand ViewportPanCommand
        {
            get { return (ViewportCalculatorViewModel.PanDelegateCommand)GetValue(ViewportPanCommandProperty); }
            set { SetValue(ViewportPanCommandProperty, value); }
        }

        public ViewportCalculatorViewModel.ZoomDelegateCommand ViewportZoomCommand
        {
            get { return (ViewportCalculatorViewModel.ZoomDelegateCommand)GetValue(ViewportZoomCommandProperty); }
            set { SetValue(ViewportZoomCommandProperty, value); }
        }

        public ViewportCalculatorViewModel.ZoomToContentDelegateCommand ViewportZoomToContentCommand
        {
            get { return (ViewportCalculatorViewModel.ZoomToContentDelegateCommand)GetValue(ViewportZoomToContentCommandProperty); }
            set { SetValue(ViewportZoomToContentCommandProperty, value); }
        }

        public DelegateCommand UnfocusAllCommand
        {
            get { return (DelegateCommand)GetValue(UnfocusAllCommandProperty); }
            set { SetValue(UnfocusAllCommandProperty, value); }
        }
    }
}
