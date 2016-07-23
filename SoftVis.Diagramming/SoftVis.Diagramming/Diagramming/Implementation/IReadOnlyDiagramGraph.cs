using Codartis.SoftVis.Graphs;
using QuickGraph;

namespace Codartis.SoftVis.Diagramming.Implementation
{
    /// <summary>
    /// A read only view of the diagram graph.
    /// </summary>
    internal interface IReadOnlyDiagramGraph : IBidirectionalGraph<DiagramNode, DiagramConnector>, 
        INotifyGraphChange<DiagramNode, DiagramConnector>
    {
    }
}
