using System;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling.Definition;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Implementation
{
    /// <summary>
    /// An immutable implementation of a diagram connector.
    /// </summary>
    public sealed class DiagramConnector : DiagramShapeBase, IDiagramConnector
    {
        public ModelNodeId Source { get; }
        public ModelNodeId Target { get; }
        public IModelRelationship ModelRelationship { get; }
        public ConnectorType ConnectorType { get; }
        public Route Route { get; }

        public DiagramConnector(
            IModelRelationship relationship,
            ModelNodeId source,
            ModelNodeId target,
            ConnectorType connectorType)
            : this(relationship, source, target, connectorType, Route.Empty)
        {
        }

        public DiagramConnector(
            IModelRelationship relationship,
            ModelNodeId source,
            ModelNodeId target,
            ConnectorType connectorType,
            Route route)
        {
            ModelRelationship = relationship ?? throw new ArgumentNullException(nameof(relationship));
            Source = source;
            Target = target;
            ConnectorType = connectorType ?? throw new ArgumentNullException(nameof(connectorType));
            Route = route;
        }

        public override Rect2D Rect => Rect2D.Zero.Union(Route);

        public ModelRelationshipId Id => ModelRelationship.Id;
        public ModelRelationshipStereotype Stereotype => ModelRelationship.Stereotype;

        public IDiagramConnector WithModelRelationship(IModelRelationship newModelRelationship)
            => CreateInstance(newModelRelationship, Source, Target, ConnectorType, Route);

        public IDiagramConnector WithConnectorType(ConnectorType newConnectorType)
            => CreateInstance(ModelRelationship, Source, Target, newConnectorType, Route);

        public IDiagramConnector WithRoute(Route newRoute) => CreateInstance(ModelRelationship, Source, Target, ConnectorType, newRoute);

        public override string ToString() => Source + "---" + ModelRelationship.Stereotype + "-->" + Target;

        [NotNull]
        private static IDiagramConnector CreateInstance(
            IModelRelationship modelRelationship,
            ModelNodeId source,
            ModelNodeId target,
            ConnectorType connectorType,
            Route route)
            => new DiagramConnector(modelRelationship, source, target, connectorType, route);
    }
}