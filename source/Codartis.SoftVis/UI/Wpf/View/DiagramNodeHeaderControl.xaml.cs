using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Codartis.SoftVis.UI.Wpf.View
{
    public partial class DiagramNodeHeaderControl : UserControl
    {
        public static readonly DependencyProperty DiagramFillProperty =
            DiagramVisual.DiagramFillProperty.AddOwner(typeof(DiagramNodeHeaderControl));

        public static readonly DependencyProperty DiagramStrokeProperty =
            DiagramVisual.DiagramStrokeProperty.AddOwner(typeof(DiagramNodeHeaderControl));

        public static readonly DependencyProperty ActualSizeProperty =
            DependencyProperty.Register("ActualSize", typeof(Size), typeof(DiagramNodeHeaderControl),
                new FrameworkPropertyMetadata(Size.Empty));

        public DiagramNodeHeaderControl()
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

        public Size ActualSize
        {
            get { return (Size)GetValue(ActualSizeProperty); }
            set { SetValue(ActualSizeProperty, value); }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            ActualSize = new Size(ActualWidth, ActualHeight);
        }
    }
}
