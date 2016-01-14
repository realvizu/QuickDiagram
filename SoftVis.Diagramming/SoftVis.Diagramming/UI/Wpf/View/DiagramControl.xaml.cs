using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// Interaction logic for DiagramControl.xaml
    /// </summary>
    public partial class DiagramControl : UserControl
    {
        private const double MinZoomDefault = .2d;
        private const double MaxZoomDefault = 5d;
        private const double InitialZoomDefault = 1d;
        private const double PanAndZoomControlSizeDefault = 120d;

        public static readonly DependencyProperty MinZoomProperty =
            DependencyProperty.Register("MinZoom", typeof(double), typeof(DiagramControl),
                new FrameworkPropertyMetadata(MinZoomDefault));

        public static readonly DependencyProperty MaxZoomProperty =
            DependencyProperty.Register("MaxZoom", typeof(double), typeof(DiagramControl),
                new FrameworkPropertyMetadata(MaxZoomDefault));

        public static readonly DependencyProperty InitialZoomProperty =
            DependencyProperty.Register("InitialZoom", typeof(double), typeof(DiagramControl),
                new PropertyMetadata(InitialZoomDefault));

        public static readonly DependencyProperty PanAndZoomControlHeightProperty =
            DependencyProperty.Register("PanAndZoomControlHeight", typeof(double), typeof(DiagramControl),
                new PropertyMetadata(PanAndZoomControlSizeDefault));

        public DiagramControl()
        {
            InitializeComponent();
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

        public double InitialZoom
        {
            get { return (double)GetValue(InitialZoomProperty); }
            set { SetValue(InitialZoomProperty, value); }
        }

        public double PanAndZoomControlHeight
        {
            get { return (double)GetValue(PanAndZoomControlHeightProperty); }
            set { SetValue(PanAndZoomControlHeightProperty, value); }
        }

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            Keyboard.Focus(DiagramViewportControl);
        }
    }
}
