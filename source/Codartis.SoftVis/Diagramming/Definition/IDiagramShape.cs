using Codartis.SoftVis.Geometry;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Definition
{
    /// <summary>
    /// A diagram shape is a representation of a model item on a diagram.
    /// Nodes and connectors are all shapes.
    /// </summary>
    public interface IDiagramShape
    {
        [NotNull] string ShapeId { get; }
        Rect2D Rect { get; }
    }
}