using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Codartis.SoftVis.Util.UI.Wpf.Commands;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// Interaction logic for DiagramNodeControl.xaml
    /// </summary>
    public partial class DiagramNodeControl : UserControl
    {
        public static readonly DependencyProperty DiagramFillProperty =
            DiagramVisual.DiagramFillProperty.AddOwner(typeof(DiagramNodeControl));

        public static readonly DependencyProperty DiagramStrokeProperty =
            DiagramVisual.DiagramStrokeProperty.AddOwner(typeof(DiagramNodeControl));

        public static readonly DependencyProperty ActualSizeProperty =
            DependencyProperty.Register("ActualSize", typeof(Size), typeof(DiagramNodeControl),
                new FrameworkPropertyMetadata(Size.Empty));

        public static readonly DependencyProperty FocusDiagramItemCommandProperty =
            DependencyProperty.Register("FocusDiagramItemCommand", typeof(DelegateCommand), typeof(DiagramNodeControl));

        public DiagramNodeControl()
        {
            InitializeComponent();
            Focusable = true;
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

        public DelegateCommand FocusDiagramItemCommand
        {
            get { return (DelegateCommand)GetValue(FocusDiagramItemCommandProperty); }
            set { SetValue(FocusDiagramItemCommandProperty, value); }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            FocusDiagramItemCommand?.Execute();

            e.Handled = true;
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            ActualSize = new Size(ActualWidth, ActualHeight);
        }
    }
}
