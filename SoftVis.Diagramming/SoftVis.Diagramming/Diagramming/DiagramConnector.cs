using System;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;
using QuickGraph;

namespace Codartis.SoftVis.Diagramming
{
    /// <summary>
    /// A diagram connector is an edge in the diagram graph.
    /// It is the representation of a directed model relationship and it connects two diagram nodes.
    /// Eg. an inheritance arrow pointing from a derived class shape to its base class shape.
    /// </summary>
    public abstract class DiagramConnector : DiagramShape, IEdge<DiagramNode>, IEdge<IRect>, IEdge<ISized>
    {
        public DiagramNode Source { get; }
        public DiagramNode Target { get; }
        public virtual Route RoutePoints { get; set; }

        IRect IEdge<IRect>.Target => Target;
        IRect IEdge<IRect>.Source => Source;

        ISized IEdge<ISized>.Target => Target;
        ISized IEdge<ISized>.Source => Source;

        protected DiagramConnector(IModelRelationship relationship, DiagramNode source, DiagramNode target)
            : base(relationship)
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
