using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Codartis.SoftVis.UI.Wpf.ViewModel;
using Codartis.SoftVis.Util.UI.Wpf.Commands;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// Interaction logic for ContainerDiagramNodeControl.xaml
    /// </summary>
    public partial class ContainerDiagramNodeControl : UserControl
    {
        public static readonly DependencyProperty DiagramFillProperty =
            DiagramVisual.DiagramFillProperty.AddOwner(typeof(ContainerDiagramNodeControl));

        public static readonly DependencyProperty DiagramStrokeProperty =
            DiagramVisual.DiagramStrokeProperty.AddOwner(typeof(ContainerDiagramNodeControl));

        public static readonly DependencyProperty ActualSizeProperty =
            DependencyProperty.Register("ActualSize", typeof(Size), typeof(ContainerDiagramNodeControl),
                new FrameworkPropertyMetadata(Size.Empty));

        public static readonly DependencyProperty FocusRequestedCommandProperty =
            DependencyProperty.Register("FocusRequestedCommand", typeof(DelegateCommand<DiagramShapeViewModelBase>), typeof(ContainerDiagramNodeControl));

        public ContainerDiagramNodeControl()
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

        public DelegateCommand<DiagramShapeViewModelBase> FocusRequestedCommand
        {
            get { return (DelegateCommand<DiagramShapeViewModelBase>)GetValue(FocusRequestedCommandProperty); }
            set { SetValue(FocusRequestedCommandProperty, value); }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            FocusRequestedCommand?.Execute(DataContext as DiagramShapeViewModelBase);

            // Must stop the event from bubbling up because if its viewport parent receives MouseMove then it forces the node to lose focus.
            e.Handled = true;
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            ActualSize = new Size(ActualWidth, ActualHeight);
        }
    }
}
