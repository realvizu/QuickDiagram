using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Diagramming
{
    /// <summary>
    /// A diagram shape is a representation of a model item on a diagram.
    /// Nodes and connectors are all shapes.
    /// </summary>
    public interface IDiagramShape
    {
        IModelItem ModelItem { get; }

        bool IsRectDefined { get; }
        Rect2D Rect { get; }

        void Update(IModelItem modelItem);
    }
}