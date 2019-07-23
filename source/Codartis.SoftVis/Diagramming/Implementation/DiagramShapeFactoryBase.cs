using System;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Diagramming.Implementation
{
    /// <summary>
    /// Creates diagram shape objects.
    /// </summary>
    public abstract class DiagramShapeFactoryBase : IDiagramShapeFactory
    {
        public abstract IDiagramNode CreateDiagramNode(IDiagramShapeResolver diagramShapeResolver, IModelNode modelNode, IModelNode parentModelNode);

        public virtual DiagramConnectorSpecification CreateDiagramConnectorSpec(
            IDiagramShapeResolver diagramShapeResolver,
            IModelRelationship modelRelationship)
        {
            if (modelRelationship == null)
                throw new ArgumentNullException(nameof(modelRelationship));

            var connectorType = diagramShapeResolver.GetConnectorType(modelRelationship.Stereotype);
            return new DiagramConnectorSpecification(modelRelationship, connectorType);
        }
    }
}