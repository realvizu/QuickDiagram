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
        public IModelRelationship ModelRelationship { get; }
        public ModelNodeId Source { get; }
        public ModelNodeId Target { get; }
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

        public override bool IsRectDefined => Route.IsDefined;
        public override Rect2D Rect => Rect2D.Zero.Union(Route);

        public ModelRelationshipId Id => ModelRelationship.Id;
        public ModelRelationshipStereotype Stereotype => ModelRelationship.Stereotype;

        public IDiagramConnector WithRoute(Route newRoute) => CreateInstance(Source, Target, newRoute);

        public override string ToString() => Source + "---" + ModelRelationship.Stereotype + "-->" + Target;

        [NotNull]
        private IDiagramConnector CreateInstance(ModelNodeId source, ModelNodeId target, Route route)
            => new DiagramConnector(ModelRelationship, source, target, ConnectorType, route);
    }
}