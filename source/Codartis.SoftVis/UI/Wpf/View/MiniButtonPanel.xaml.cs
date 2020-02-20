using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Codartis.SoftVis.UI.Wpf.View
{
    public partial class MiniButtonPanel : UserControl
    {
        public static readonly DependencyProperty DiagramFillProperty =
            DiagramVisual.DiagramFillProperty.AddOwner(typeof(MiniButtonPanel));

        public static readonly DependencyProperty DiagramStrokeProperty =
            DiagramVisual.DiagramStrokeProperty.AddOwner(typeof(MiniButtonPanel));

        public static readonly DependencyProperty DecoratedElementProperty =
            DependencyProperty.Register("DecoratedElement", typeof(UIElement), typeof(MiniButtonPanel));

        public MiniButtonPanel()
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

        public UIElement DecoratedElement
        {
            get { return (UIElement)GetValue(DecoratedElementProperty); }
            set { SetValue(DecoratedElementProperty, value); }
        }
    }
}