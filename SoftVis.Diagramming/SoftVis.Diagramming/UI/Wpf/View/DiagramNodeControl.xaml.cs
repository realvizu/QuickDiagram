using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Codartis.SoftVis.UI.Wpf.Commands;

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

        public static readonly RoutedEvent DiagramItemGotFocusRoutedEvent =
            EventManager.RegisterRoutedEvent("DiagramItemGotFocus", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(DiagramNodeControl));

        public static readonly RoutedEvent DiagramItemLostFocusRoutedEvent =
            EventManager.RegisterRoutedEvent("DiagramItemLostFocus", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(DiagramNodeControl));

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

        public void FocusDiagramItem()
        {
            FocusDiagramItemCommand?.Execute();
            RaiseDiagramItemGotFocus();
        }

        public void UnfocusDiagramItem()
        {
            UnfocusDiagramItemCommand?.Execute();
            RaiseDiagramItemLostFocus();
        }

        private void RaiseDiagramItemGotFocus()
        {
            var newEventArgs = new DiagramItemRoutedEventArgs(DiagramItemGotFocusRoutedEvent, this);
            RaiseEvent(newEventArgs);
        }

        private void RaiseDiagramItemLostFocus()
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
