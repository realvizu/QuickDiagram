using System;
using System.Collections.Generic;
using Codartis.SoftVis.Modeling;
using QuickGraph;

namespace Codartis.SoftVis.Diagramming.Shapes
{
    /// <summary>
    /// A diagram connector is an edge in the diagram graph.
    /// It is the representation of a directed model relationship and it connects two diagram nodes.
    /// Eg. an inheritance arrow pointing from a derived class shape to its base class shape.
    /// </summary>
    public abstract class DiagramConnector : DiagramShape, IEdge<DiagramNode>
    {
        public DiagramNode Source { get; }
        public DiagramNode Target { get; }
        public IEnumerable<DiagramPoint> RoutePoints { get; set; }

        protected DiagramConnector(IModelRelationship relationship, DiagramNode source, DiagramNode target)
            :base(relationship)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (target == null) throw new ArgumentNullException(nameof(target));

            Source = source;
            Target = target;
        }

        public IModelRelationship ModelRelationship => (IModelRelationship)ModelItem;
        public ModelRelationshipType Type => ModelRelationship.Type;

        public override string ToString()
        {
            return Source + "---" + Type + "-->" + Target;
        }
    }
}
