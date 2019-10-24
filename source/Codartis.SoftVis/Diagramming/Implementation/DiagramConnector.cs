using Codartis.SoftVis.Diagramming.Definition;
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
        public IModelRelationship ModelRelationship { get; }
        public ConnectorType ConnectorType { get; }
        public Route Route { get; }

        public DiagramConnector(
            [NotNull] IModelRelationship relationship,
            ConnectorType connectorType)
            : this(relationship, connectorType, Route.Empty)
        {
        }

        public DiagramConnector(
            [NotNull] IModelRelationship relationship,
            ConnectorType connectorType,
            Route route)
        {
            ModelRelationship = relationship;
            ConnectorType = connectorType;
            Route = route;
        }

        public ModelRelationshipId Id => ModelRelationship.Id;
        public ModelRelationshipStereotype Stereotype => ModelRelationship.Stereotype;
        public ModelNodeId Source => ModelRelationship.Source;
        public ModelNodeId Target => ModelRelationship.Target;

        public override string ShapeId => ModelRelationship.Id.ToShapeId();
        public override Rect2D Rect => Rect2D.Zero.Union(Route);

        public IDiagramConnector WithModelRelationship(IModelRelationship newModelRelationship) => CreateInstance(newModelRelationship, ConnectorType, Route);
        public IDiagramConnector WithConnectorType(ConnectorType newConnectorType) => CreateInstance(ModelRelationship, newConnectorType, Route);
        public IDiagramConnector WithRoute(Route newRoute) => CreateInstance(ModelRelationship, ConnectorType, newRoute);

        public override string ToString() => Source + "---" + Stereotype + "-->" + Target;

        [NotNull]
        private static IDiagramConnector CreateInstance(
            [NotNull] IModelRelationship modelRelationship,
            ConnectorType connectorType,
            Route route)
            => new DiagramConnector(modelRelationship, connectorType, route);
    }
}