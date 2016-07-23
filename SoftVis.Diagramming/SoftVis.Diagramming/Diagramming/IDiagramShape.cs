using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Diagramming
{
    public interface IDiagramShape
    {
        IModelItem ModelItem { get; }
        Rect2D Rect { get; }
    }
}