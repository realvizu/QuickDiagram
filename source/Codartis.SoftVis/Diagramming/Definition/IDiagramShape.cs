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

        /// <summary>
        /// Bounding rectangle of the shape on teh canvas.
        /// </summary>
        Rect2D AbsoluteRect { get; }

        /// <summary>
        /// Bounding rectangle of the shape relative to its parent's ChildrenArea.
        /// If no parent then the same as the <see cref="AbsoluteRect"/>.
        /// </summary>
        Rect2D RelativeRect { get; }
    }
}