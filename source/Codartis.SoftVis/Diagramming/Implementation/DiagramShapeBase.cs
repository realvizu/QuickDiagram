using System;
using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Diagramming.Implementation
{
    /// <summary>
    /// The common base type for all diagram shapes (nodes and connectors as well).
    /// </summary>
    [Immutable]
    public abstract class DiagramShapeBase : IDiagramShape
    {
        public abstract bool IsRectDefined { get; }
        public abstract Rect2D Rect { get; }
    }
}
