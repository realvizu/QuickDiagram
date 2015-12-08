using Codartis.SoftVis.Diagramming.Graph;
using Codartis.SoftVis.Rendering.Extensibility;

namespace Codartis.SoftVis.Rendering.Wpf
{
    /// <summary>
    /// The view model of a DiagramViewerControl.
    /// </summary>
    public class DiagramViewerViewModel
    {
        public Diagram Diagram { get; }
        public IDiagramBehaviourProvider DiagramBehaviourProvider { get; }

        public DiagramViewerViewModel(Diagram diagram, IDiagramBehaviourProvider diagramBehaviourProvider)
        {
            Diagram = diagram;
            DiagramBehaviourProvider = diagramBehaviourProvider;
        }
    }
}
