using System;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Graphs.Immutable;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.Util;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming
{
    /// <summary>
    /// A diagram node is a vertex in the diagram graph that represents a model node (eg. a box representing a class).
    /// All diagram nodes have a name, a size and a position.
    /// A diagram node can have a parent node that contains it.
    /// Immutable.
    /// </summary>
    /// <remarks>
    /// The name plays role in layout as a basis for ordering.
    /// </remarks>
    public interface IDiagramNode : IImmutableVertex<ModelNodeId>, IDiagramShape, IComparable<IDiagramNode>
    {
        [NotNull] IModelNode ModelNode { get; }
        DateTime AddedAt { get; }
        Maybe<ModelNodeId> ParentNodeId { get; }

        bool HasParent { get; }
        [NotNull] string Name { get; }
        Size2D Size { get; }
        Point2D Center { get; }
        Point2D TopLeft { get; }

        [NotNull]
        IDiagramNode WithModelNode([NotNull] IModelNode newModelNode);

        [NotNull]
        IDiagramNode WithParentNodeId(Maybe<ModelNodeId> newParentNodeId);

        [NotNull]
        IDiagramNode WithRect(Rect2D newRect);

        [NotNull]
        IDiagramNode WithSize(Size2D newSize);

        [NotNull]
        IDiagramNode WithCenter(Point2D newCenter);

        [NotNull]
        IDiagramNode WithTopLeft(Point2D newTopLeft);
    }
}