using Codartis.SoftVis.Modeling.Definition;

namespace Codartis.SoftVis.Diagramming
{
    /// <summary>
    /// Responsible for creating diagram node and connector objects.
    /// </summary>
    public interface IDiagramShapeFactory
    {
        IDiagramNode CreateDiagramNode(IDiagramShapeResolver diagramShapeResolver, IModelNode modelNode, IModelNode parentModelNode);
        IDiagramConnector CreateDiagramConnector(IDiagramShapeResolver diagramShapeResolver, IModelRelationship modelRelationship);
    }
}