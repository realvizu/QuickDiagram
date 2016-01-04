using System.Windows;

namespace Codartis.SoftVis.UI.Wpf.InputControls
{
    internal partial class PanAndZoomControl
    {
        public static readonly DependencyProperty MinZoomProperty =
            DependencyProperty.Register("MinZoom", typeof(double), typeof(PanAndZoomControl));

        public static readonly DependencyProperty MaxZoomProperty =
            DependencyProperty.Register("MaxZoom", typeof(double), typeof(PanAndZoomControl));

        public static readonly DependencyProperty ZoomValueProperty =
            DependencyProperty.Register("ZoomValue", typeof(double), typeof(PanAndZoomControl), 
                new PropertyMetadata(ZoomValueChanged));

        public static readonly DependencyProperty LinearZoomValueProperty =
            DependencyProperty.Register("LinearZoomValue", typeof(double), typeof(PanAndZoomControl), 
                new PropertyMetadata(LinearZoomValueChanged));

        public static readonly DependencyProperty SmallIncrementProperty =
            DependencyProperty.Register("SmallIncrement", typeof(double), typeof(PanAndZoomControl));

        public static readonly DependencyProperty LargeIncrementProperty =
            DependencyProperty.Register("LargeIncrement", typeof(double), typeof(PanAndZoomControl));
    }
}