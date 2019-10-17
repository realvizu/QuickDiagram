using System;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Graphs.Immutable;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.Util;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Definition
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
    public interface IDiagramNode : IDiagramShape, IImmutableVertex<ModelNodeId>, IComparable<IDiagramNode>
    {
        [NotNull] IModelNode ModelNode { get; }
        Maybe<ModelNodeId> ParentNodeId { get; }
        bool HasParent { get; }
        [NotNull] string Name { get; }
        DateTime AddedAt { get; }
        Point2D Center { get; }
        Point2D TopLeft { get; }
        Size2D PayloadAreaSize { get; }
        Size2D ChildrenAreaSize { get; }
        Size2D Size { get; }

        [NotNull]
        IDiagramNode WithModelNode([NotNull] IModelNode newModelNode);

        [NotNull]
        IDiagramNode WithParentNodeId(ModelNodeId? newParentNodeId);

        [NotNull]
        IDiagramNode WithCenter(Point2D newCenter);

        [NotNull]
        IDiagramNode WithTopLeft(Point2D newTopLeft);

        [NotNull]
        IDiagramNode WithPayloadAreaSize(Size2D newSize);

        [NotNull]
        IDiagramNode WithChildrenAreaSize(Size2D newSize);
    }
}