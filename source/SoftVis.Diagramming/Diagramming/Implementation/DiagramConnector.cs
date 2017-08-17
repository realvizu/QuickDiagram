using System;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Util;
using QuickGraph;

namespace Codartis.SoftVis.Diagramming.Implementation
{
    /// <summary>
    /// A diagram connector is an edge in the diagram graph.
    /// It is the representation of a directed model relationship and it connects two diagram nodes.
    /// Eg. an inheritance arrow pointing from a derived class shape to its base class shape.
    /// </summary>
    public class DiagramConnector : DiagramShape, IDiagramConnector, IEdge<IDiagramNode>
    {
        private Route _routePoints;

        public IModelRelationship ModelRelationship { get; private set; }
        public IDiagramNode Source { get; }
        public IDiagramNode Target { get; }
        public ConnectorType ConnectorType { get; }

        public event Action<IDiagramConnector, Route, Route> RouteChanged;

        public DiagramConnector(IModelRelationship relationship, IDiagramNode source, IDiagramNode target, ConnectorType connectorType)
            : base(relationship)
        {
            ModelRelationship = relationship;
            Source = source ?? throw new ArgumentNullException(nameof(source));
            Target = target ?? throw new ArgumentNullException(nameof(target));
            ConnectorType = connectorType;
        }

        public virtual Route RoutePoints
        {
            get { return _routePoints; }
            set
            {
                if (!_routePoints.EmptyIfNullSequenceEqual(value))
                {
                    var oldRoute = _routePoints;
                    _routePoints = value;
                    RouteChanged?.Invoke(this, oldRoute, value);
                }
            }
        }

        public override bool IsRectDefined => Source.IsRectDefined && Target.IsRectDefined && RoutePoints != null;
        public override Rect2D Rect => CalculateRect(Source.Rect, Target.Rect, RoutePoints);

        public override void Update(IModelItem modelItem)
        {
            base.Update(modelItem);

            if (modelItem is IModelRelationship modelRelationship)
            {
                ModelRelationship = modelRelationship;
            }
            else
            {
                throw new ArgumentException($"IModelRelationship expected but received {modelItem.GetType().Name}");
            }
        }

        private static Rect2D CalculateRect(Rect2D sourceRect, Rect2D targetRect, Route routePoints)
        {
            var rectUnion = sourceRect.Union(targetRect);
            if (routePoints != null)
            {
                foreach (var routePoint in routePoints)
                    rectUnion = rectUnion.Union(routePoint);
            }
            return rectUnion;
        }

        public override string ToString() => Source + "---" + ModelRelationship.Stereotype + "-->" + Target;
    }
}
