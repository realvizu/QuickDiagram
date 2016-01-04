using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using Codartis.SoftVis.UI.Common;
using Codartis.SoftVis.UI.Wpf.Commands;

namespace Codartis.SoftVis.UI.Wpf.Behaviors
{
    internal abstract class PanAndZoomBehaviorBase : Behavior<FrameworkElement>
    {
        public static readonly DependencyProperty PanCommandProperty =
            DependencyProperty.Register("PanCommand", typeof(ICommand), typeof(PanAndZoomBehaviorBase));

        public static readonly DependencyProperty ZoomCommandProperty =
            DependencyProperty.Register("ZoomCommand", typeof(ICommand), typeof(PanAndZoomBehaviorBase));

        public ICommand PanCommand
        {
            get { return (ICommand)GetValue(PanCommandProperty); }
            set { SetValue(PanCommandProperty, value); }
        }

        public ICommand ZoomCommand
        {
            get { return (ICommand)GetValue(ZoomCommandProperty); }
            set { SetValue(ZoomCommandProperty, value); }
        }

        protected void Pan(Vector vector)
        {
            PanCommand?.Execute(vector);
        }

        protected void Zoom(ZoomDirection direction, double amount, Point center)
        {
            var commandParameters = new ZoomCommandParameters(direction, amount, center);
            ZoomCommand?.Execute(commandParameters);
        }
    }
}
