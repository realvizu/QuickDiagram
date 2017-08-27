using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Diagramming
{
    /// <summary>
    /// Responsible for creating diagram node and connector objects.
    /// </summary>
    public interface IDiagramShapeFactory
    {
        IDiagramNode CreateDiagramNode(IDiagramShapeResolver diagramShapeResolver, IModelNode modelNode);
        IDiagramConnector CreateDiagramConnector(IDiagramShapeResolver diagramShapeResolver, IModelRelationship modelRelationship);
    }
}