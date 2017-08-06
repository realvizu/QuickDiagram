using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling2;

namespace Codartis.SoftVis.Diagramming.Implementation
{
    /// <summary>
    /// The common base type for all diagram shapes (nodes and connectors as well).
    /// </summary>
    public abstract class DiagramShape : IDiagramShape
    {
        public ModelItemId ModelItemId { get; }

        protected DiagramShape(ModelItemId modelItemId)
        {
            ModelItemId = modelItemId;
        }

        public abstract bool IsRectDefined { get; }
        public abstract Rect2D Rect { get; }
    }
}
