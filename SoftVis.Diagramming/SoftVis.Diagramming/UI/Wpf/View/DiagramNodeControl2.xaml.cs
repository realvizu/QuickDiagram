using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Codartis.SoftVis.UI.Wpf.Commands;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// Interaction logic for DiagramNodeControl2.xaml
    /// </summary>
    public partial class DiagramNodeControl2 : UserControl
    {
        public static readonly DependencyProperty DiagramFillProperty =
            DiagramVisual.DiagramFillProperty.AddOwner(typeof(DiagramNodeControl2));

        public static readonly DependencyProperty DiagramStrokeProperty =
            DiagramVisual.DiagramStrokeProperty.AddOwner(typeof(DiagramNodeControl2));

        public static readonly DependencyProperty FocusCommandProperty =
            DependencyProperty.Register("FocusCommand", typeof(DelegateCommand), typeof(DiagramNodeControl2));

        public static readonly DependencyProperty UnfocusCommandProperty =
            DependencyProperty.Register("UnfocusCommand", typeof(DelegateCommand), typeof(DiagramNodeControl2));

        public static readonly RoutedEvent DiagramItemGotFocusRoutedEvent =
            EventManager.RegisterRoutedEvent("DiagramItemGotFocus", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(DiagramNodeControl2));

        public static readonly RoutedEvent DiagramItemLostFocusRoutedEvent =
            EventManager.RegisterRoutedEvent("DiagramItemLostFocus", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(DiagramNodeControl2));

        public DiagramNodeControl2()
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

        public DelegateCommand FocusCommand
        {
            get { return (DelegateCommand)GetValue(FocusCommandProperty); }
            set { SetValue(FocusCommandProperty, value); }
        }

        public DelegateCommand UnfocusCommand
        {
            get { return (DelegateCommand)GetValue(UnfocusCommandProperty); }
            set { SetValue(UnfocusCommandProperty, value); }
        }

        public event RoutedEventHandler DiagramItemGotFocus
        {
            add { AddHandler(DiagramItemGotFocusRoutedEvent, value); }
            remove { RemoveHandler(DiagramItemGotFocusRoutedEvent, value); }
        }

        public event RoutedEventHandler DiagramItemLostFocus
        {
            add { AddHandler(DiagramItemLostFocusRoutedEvent, value); }
            remove { RemoveHandler(DiagramItemLostFocusRoutedEvent, value); }
        }

        public void RaiseDiagramItemGotFocus()
        {
            var newEventArgs = new DiagramItemRoutedEventArgs(DiagramItemGotFocusRoutedEvent, this);
            RaiseEvent(newEventArgs);
        }

        public void RaiseDiagramItemLostFocus()
        {
            var newEventArgs = new DiagramItemRoutedEventArgs(DiagramItemLostFocusRoutedEvent, this);
            RaiseEvent(newEventArgs);
        }

        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        {
            return new PointHitTestResult(this, hitTestParameters.HitPoint);
        }
    }
}
