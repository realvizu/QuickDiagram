using System;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Graphs.Immutable;
using Codartis.SoftVis.Modeling.Definition;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming
{
    /// <summary>
    /// A diagram node is a vertex in the diagram graph that represents a model node (eg. a box representing a class).
    /// All diagram nodes have a name, a size and a position.
    /// Immutable.
    /// </summary>
    /// <remarks>
    /// The name plays role in layout as a basis for ordering.
    /// </remarks>
    public interface IDiagramNode : IImmutableVertex<ModelNodeId>, IDiagramShape, IComparable<IDiagramNode>
    {
        [NotNull] IModelNode ModelNode { get; }
        DateTime AddedAt { get; }

        [NotNull] string Name { get; }
        Size2D Size { get; }
        Point2D Center { get; }
        Point2D TopLeft { get; }
        ModelNodeId? ParentNodeId { get; }
        bool HasParent { get; }

        [NotNull]
        IDiagramNode WithModelNode(IModelNode newModelNode);

        [NotNull]
        IDiagramNode WithRect(Rect2D newRect);

        [NotNull]
        IDiagramNode WithSize(Size2D newSize);

        [NotNull]
        IDiagramNode WithCenter(Point2D newCenter);

        [NotNull]
        IDiagramNode WithTopLeft(Point2D newTopLeft);

        [NotNull]
        IDiagramNode WithParentNodeId(ModelNodeId? newParentNodeId);
    }
}