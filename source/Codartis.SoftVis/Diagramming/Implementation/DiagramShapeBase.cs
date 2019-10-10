using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Diagramming.Implementation
{
    /// <summary>
    /// The common base type for all diagram shapes (nodes and connectors as well).
    /// Immutable.
    /// </summary>
    public abstract class DiagramShapeBase : IDiagramShape
    {
        public abstract string ShapeId { get; }
        public abstract Rect2D Rect { get; }
    }
}
