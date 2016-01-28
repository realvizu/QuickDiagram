using System.Windows;

namespace Codartis.SoftVis.UI.Wpf.View
{
    public class DiagramItemRoutedEventArgs : RoutedEventArgs
    {
        public DiagramNodeControl2 DiagramNodeControl { get; }

        public DiagramItemRoutedEventArgs(RoutedEvent routedEvent, DiagramNodeControl2 diagramNodeControl) 
            : base(routedEvent)
        {
            DiagramNodeControl = diagramNodeControl;
        }
    }
}
