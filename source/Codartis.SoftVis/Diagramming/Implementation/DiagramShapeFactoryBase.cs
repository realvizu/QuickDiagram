using System;
using Codartis.SoftVis.Modeling.Definition;

namespace Codartis.SoftVis.Diagramming.Implementation
{
    /// <summary>
    /// Creates diagram shape objects.
    /// </summary>
    public abstract class DiagramShapeFactoryBase : IDiagramShapeFactory
    {
        public abstract IDiagramNode CreateDiagramNode(IDiagramShapeResolver diagramShapeResolver, IModelNode modelNode, IModelNode parentModelNode);

        public virtual IDiagramConnector CreateDiagramConnector(
            IDiagramShapeResolver diagramShapeResolver,
            IModelRelationship modelRelationship)
        {
            if (modelRelationship == null)
                throw new ArgumentNullException(nameof(modelRelationship));

            var sourceNode = diagramShapeResolver.GetDiagramNode(modelRelationship.Source.Id);
            var targetNode = diagramShapeResolver.GetDiagramNode(modelRelationship.Target.Id);
            var connectorType = diagramShapeResolver.GetConnectorType(modelRelationship.Stereotype);
            return new DiagramConnector(modelRelationship, sourceNode, targetNode, connectorType);
        }
    }
}