using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Diagramming
{
    /// <summary>
    /// Responsible for creating diagram node and connector objects.
    /// </summary>
    public interface IDiagramShapeFactory
    {
        IDiagramNode CreateDiagramNode(IModelNode modelNode);
        IDiagramConnector CreateDiagramConnector(IModelRelationship modelRelationship);
    }
}