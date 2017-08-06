using System;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling2;
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

        public IDiagramNode Source { get; }
        public IDiagramNode Target { get; }
        public string RelationshipType { get; }

        public event Action<IDiagramConnector, Route, Route> RouteChanged;

        public DiagramConnector(IModelRelationship relationship, IDiagramNode source, IDiagramNode target)
            : base(relationship.Id)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
            Target = target ?? throw new ArgumentNullException(nameof(target));
            RelationshipType = relationship.GetType().Name;
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

        //public IModelRelationship ModelRelationship => (IModelRelationship)ModelItem;
        //public ModelRelationshipClassifier Classifier => ModelRelationship.Classifier;
        //public ModelRelationshipStereotype Stereotype => ModelRelationship.Stereotype;
        //public ModelRelationshipType Type => ModelRelationship.Type;

        public override bool IsRectDefined => Source.IsRectDefined && Target.IsRectDefined && RoutePoints != null;
        public override Rect2D Rect => CalculateRect(Source.Rect, Target.Rect, RoutePoints);

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

        public override string ToString() => Source + "---" + RelationshipType + "-->" + Target;
    }
}
