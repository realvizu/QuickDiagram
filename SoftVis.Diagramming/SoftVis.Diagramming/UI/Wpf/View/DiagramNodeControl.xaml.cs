using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Codartis.SoftVis.Util.UI.Wpf;

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

        public static readonly DependencyProperty FocusDiagramItemCommandProperty =
            DependencyProperty.Register("FocusDiagramItemCommand", typeof(DelegateCommand), typeof(DiagramNodeControl));

        public static readonly DependencyProperty UnfocusDiagramItemCommandProperty =
            DependencyProperty.Register("UnfocusDiagramItemCommand", typeof(DelegateCommand), typeof(DiagramNodeControl));

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

        public DelegateCommand FocusDiagramItemCommand
        {
            get { return (DelegateCommand)GetValue(FocusDiagramItemCommandProperty); }
            set { SetValue(FocusDiagramItemCommandProperty, value); }
        }

        public DelegateCommand UnfocusDiagramItemCommand
        {
            get { return (DelegateCommand)GetValue(UnfocusDiagramItemCommandProperty); }
            set { SetValue(UnfocusDiagramItemCommandProperty, value); }
        }

        public void FocusDiagramItem()
        {
            FocusDiagramItemCommand?.Execute();
        }

        public void UnfocusDiagramItem()
        {
            UnfocusDiagramItemCommand?.Execute();
        }

        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        {
            return new PointHitTestResult(this, hitTestParameters.HitPoint);
        }
    }
}
