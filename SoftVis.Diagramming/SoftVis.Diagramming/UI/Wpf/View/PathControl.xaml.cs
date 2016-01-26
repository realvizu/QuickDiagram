using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using WpfGeometry = System.Windows.Media.Geometry;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// Wraps a Path shape into a UserControl so it can inherit Foreground and other properties.
    /// </summary>
    public partial class PathControl : UserControl
    {
        public static readonly DependencyProperty StrokeThicknessProperty = Shape.StrokeThicknessProperty.AddOwner(typeof(PathControl));
        public static readonly DependencyProperty StretchProperty = Shape.StretchProperty.AddOwner(typeof(PathControl));
        public static readonly DependencyProperty DataProperty = Path.DataProperty.AddOwner(typeof(PathControl));

        public PathControl()
        {
            InitializeComponent();
        }

        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public Stretch Stretch
        {
            get { return (Stretch)GetValue(StretchProperty); }
            set { SetValue(StretchProperty, value); }
        }

        public WpfGeometry Data
        {
            get { return (WpfGeometry)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
        }

        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        {
            return new PointHitTestResult(this, hitTestParameters.HitPoint);
        }
    }
}
