using System;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Diagramming.Implementation
{
    /// <summary>
    /// An immutable implementation of a diagram connector.
    /// </summary>
    public sealed class DiagramConnector : DiagramShapeBase, IDiagramConnector
    {
        public IModelRelationship ModelRelationship { get; }
        public IDiagramNode Source { get; }
        public IDiagramNode Target { get; }
        public ConnectorType ConnectorType { get; }
        public Route Route { get; }

        public DiagramConnector(IModelRelationship relationship, IDiagramNode source, IDiagramNode target,
            ConnectorType connectorType)
            : this(relationship, source, target, connectorType, Route.Empty)
        {
        }

        public DiagramConnector(IModelRelationship relationship, IDiagramNode source, IDiagramNode target,
            ConnectorType connectorType, Route route)
        {
            ModelRelationship = relationship ?? throw new ArgumentNullException(nameof(relationship));
            Source = source ?? throw new ArgumentNullException(nameof(source));
            Target = target ?? throw new ArgumentNullException(nameof(target));
            ConnectorType = connectorType ?? throw new ArgumentNullException(nameof(connectorType));
            Route = route;
        }

        public override bool IsRectDefined => Source.IsRectDefined && Target.IsRectDefined;
        public override Rect2D Rect => Source.Rect.Union(Target.Rect).Union(Route);

        public ModelRelationshipId Id => ModelRelationship.Id;
        public ModelRelationshipStereotype Stereotype => ModelRelationship.Stereotype;

        public IDiagramConnector WithSource(IDiagramNode newSourceNode)
        {
            if (Source.Id != newSourceNode.Id)
                throw new InvalidOperationException($"New source node must have the same id as the old one. OldId={Source.Id}, NewId={newSourceNode.Id}");

            return CreateInstance(newSourceNode, Target, Route);
        }

        public IDiagramConnector WithTarget(IDiagramNode newTargetNode)
        {
            if (Target.Id != newTargetNode.Id)
                throw new InvalidOperationException($"New target node must have the same id as the old one. OldId={Source.Id}, NewId={newTargetNode.Id}");

            return CreateInstance(Source, newTargetNode, Route);
        }

        public IDiagramConnector WithRoute(Route newRoute) => CreateInstance(Source, Target, newRoute);

        public override string ToString() => Source + "---" + ModelRelationship.Stereotype + "-->" + Target;

        private IDiagramConnector CreateInstance(IDiagramNode source, IDiagramNode target, Route route)
            => new DiagramConnector(ModelRelationship, source, target, ConnectorType, route);
    }
}
