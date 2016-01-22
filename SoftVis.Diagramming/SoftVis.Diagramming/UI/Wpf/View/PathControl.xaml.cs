using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using WpfGeometry = System.Windows.Media.Geometry;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// Interaction logic for PathControl.xaml
    /// </summary>
    public partial class PathControl : UserControl
    {
        public static readonly DependencyProperty DiagramFillProperty = DiagramVisual.DiagramFillProperty.AddOwner(typeof(PathControl));
        public static readonly DependencyProperty DiagramStrokeProperty = DiagramVisual.DiagramStrokeProperty.AddOwner(typeof(PathControl));
        public static readonly DependencyProperty StrokeThicknessProperty = Shape.StrokeThicknessProperty.AddOwner(typeof(PathControl));
        public static readonly DependencyProperty StretchProperty = Shape.StretchProperty.AddOwner(typeof(PathControl));
        public static readonly DependencyProperty DataProperty = Path.DataProperty.AddOwner(typeof(PathControl));

        public PathControl()
        {
            InitializeComponent();
        }

        public Brush DiagramFill
        {
            get { return (Brush)GetValue(DiagramFillProperty); }
            set { SetValue(DiagramFillProperty, value); }
        }

        public Brush DiagramStroke
        {
            get { return (Brush)GetValue(DiagramStrokeProperty); }
            set { SetValue(DiagramStrokeProperty, value); }
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
    }
}
