using Codartis.SoftVis.Modeling;
using QuickGraph;
using System;
using System.Collections.Generic;

namespace Codartis.SoftVis.Diagramming.Shapes
{
    /// <summary>
    /// A diagram connector is an edge in the diagram graph.
    /// It is the representation of a directed model relationship and it connects two diagram nodes.
    /// Eg. an inheritance arrow pointing from a derived class shape to its base class shape.
    /// </summary>
    public class DiagramConnector : DiagramShape, IEdge<DiagramNode>
    {
        public DiagramNode Source { get; }
        public DiagramNode Target { get; }
        public DiagramConnectorType Type { get; }
        public IEnumerable<DiagramPoint> RoutePoints { get; set; }

        public DiagramConnector(UmlRelationship umlRelationship, DiagramNode source, DiagramNode target, DiagramConnectorType type)
            :base(umlRelationship)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (target == null) throw new ArgumentNullException(nameof(target));

            Source = source;
            Target = target;
            Type = type;
        }

        public UmlRelationship UmlRelationship => (UmlRelationship)ModelElement;

        public override string ToString()
        {
            return Source + "---" + Type + "-->" + Target;
        }
    }
}
