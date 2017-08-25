using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Diagramming.Implementation
{
    /// <summary>
    /// Creates diagram shape objects.
    /// </summary>
    public class DiagramShapeFactoryBase : IDiagramShapeFactory
    {
        protected IReadOnlyDiagramStore DiagramStore { get; }
        protected IDiagramNodeResolver DiagramNodeResolver { get; }

        protected DiagramShapeFactoryBase(IReadOnlyDiagramStore diagramStore, IDiagramNodeResolver diagramNodeResolver)
        {
            DiagramStore = diagramStore;
            DiagramNodeResolver = diagramNodeResolver;
        }

        public virtual IDiagramNode CreateDiagramNode(IModelNode modelNode) 
            => new DiagramNode(modelNode);

        public IDiagramConnector CreateDiagramConnector(IModelRelationship modelRelationship)
        {
            var sourceNode = DiagramNodeResolver.GetDiagramNodeByModelNode(modelRelationship.Source);
            var targetNode = DiagramNodeResolver.GetDiagramNodeByModelNode(modelRelationship.Target);
            var connectorType = DiagramStore.GetConnectorType(modelRelationship.Stereotype);
            return new DiagramConnector(modelRelationship, sourceNode, targetNode, connectorType);
        }
    }
}
