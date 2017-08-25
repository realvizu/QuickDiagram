using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Diagramming
{
    /// <summary>
    /// Resolves diagram nodes.
    /// </summary>
    public interface IDiagramNodeResolver
    {
        IDiagramNode GetDiagramNodeByModelNode(IModelNode modelNode);
    }
}
