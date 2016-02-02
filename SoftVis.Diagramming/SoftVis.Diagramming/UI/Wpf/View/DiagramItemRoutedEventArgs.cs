using System.Windows;

namespace Codartis.SoftVis.UI.Wpf.View
{
    public class DiagramItemRoutedEventArgs : RoutedEventArgs
    {
        public DiagramNodeControl DiagramNodeControl { get; }

        public DiagramItemRoutedEventArgs(RoutedEvent routedEvent, DiagramNodeControl diagramNodeControl) 
            : base(routedEvent)
        {
            DiagramNodeControl = diagramNodeControl;
        }
    }
}
