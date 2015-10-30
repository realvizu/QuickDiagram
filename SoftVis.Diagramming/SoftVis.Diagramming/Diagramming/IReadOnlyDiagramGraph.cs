using QuickGraph;

namespace Codartis.SoftVis.Diagramming
{
    /// <summary>
    /// A read only view of the diagram graph.
    /// </summary>
    internal interface IReadOnlyDiagramGraph : IBidirectionalGraph<DiagramNode, DiagramConnector>
    {
    }
}
