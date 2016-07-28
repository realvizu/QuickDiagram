using System;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;
using QuickGraph;

namespace Codartis.SoftVis.Diagramming.Implementation
{
    /// <summary>
    /// A diagram connector is an edge in the diagram graph.
    /// It is the representation of a directed model relationship and it connects two diagram nodes.
    /// Eg. an inheritance arrow pointing from a derived class shape to its base class shape.
    /// </summary>
    public class DiagramConnector : DiagramShape, IDiagramConnector, IEdge<DiagramNode>
    {
        public DiagramNode Source { get; }
        IDiagramNode IDiagramConnector.Source => Source;

        public DiagramNode Target { get; }
        IDiagramNode IDiagramConnector.Target => Target;

        public virtual Route RoutePoints { get; set; }

        public DiagramConnector(IModelRelationship relationship, DiagramNode source, DiagramNode target)
            : base(relationship)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (target == null) throw new ArgumentNullException(nameof(target));

            Source = source;
            Target = target;
        }

        public IModelRelationship ModelRelationship => (IModelRelationship)ModelItem;
        public ModelRelationshipType Type => ModelRelationship.Type;
        public ModelRelationshipStereotype Stereotype => ModelRelationship.Stereotype;
        public override Rect2D Rect => CalculateRect(Source.Rect, Target.Rect, RoutePoints);

        public bool IsOfType(ModelRelationshipTypeSpecification typeSpecification)
            => ModelRelationship.IsOfType(typeSpecification);

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

        public override string ToString()
        {
            return Source + "---" + Type + "-->" + Target;
        }
    }
}
