using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Diagramming.Definition
{
    /// <summary>
    /// A diagram shape is a representation of a model item on a diagram.
    /// Nodes and connectors are all shapes.
    /// </summary>
    public interface IDiagramShape
    {
        Rect2D Rect { get; }
    }
}