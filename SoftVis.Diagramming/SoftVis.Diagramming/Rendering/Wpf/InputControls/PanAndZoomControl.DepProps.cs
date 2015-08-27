using System.Windows;
using System.Windows.Controls;

namespace Codartis.SoftVis.Rendering.Wpf.InputControls
{
    public partial class PanAndZoomControl : Control
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