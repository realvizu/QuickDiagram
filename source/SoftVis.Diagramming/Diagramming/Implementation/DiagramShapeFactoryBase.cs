using System;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Diagramming.Implementation
{
    /// <summary>
    /// Creates diagram shape objects.
    /// </summary>
    public class DiagramShapeFactoryBase : IDiagramShapeFactory
    {
        public virtual IDiagramNode CreateDiagramNode(IDiagramShapeResolver diagramShapeResolver, IModelNode modelNode)
        {
            if (modelNode == null)
                throw new ArgumentNullException(nameof(modelNode));

            return new DiagramNode(modelNode);
        }

        public virtual IDiagramConnector CreateDiagramConnector(IDiagramShapeResolver diagramShapeResolver, IModelRelationship modelRelationship)
        {
            if (modelRelationship == null)
                throw new ArgumentNullException(nameof(modelRelationship));

            var sourceNode = diagramShapeResolver.GetDiagramNodeById(modelRelationship.Source.Id);
            var targetNode = diagramShapeResolver.GetDiagramNodeById(modelRelationship.Target.Id);
            var connectorType = diagramShapeResolver.GetConnectorType(modelRelationship.Stereotype);
            return new DiagramConnector(modelRelationship, sourceNode, targetNode, connectorType);
        }
    }
}
