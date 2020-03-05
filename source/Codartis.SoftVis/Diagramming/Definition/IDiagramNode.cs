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

        /// <summary>
        /// Absolute position in the canvas.
        /// </summary>
        Point2D AbsoluteTopLeft { get; }

        /// <summary>
        /// Position relative to the parent's ChildrenArea.
        /// </summary>
        Point2D RelativeTopLeft { get; }

        Size2D Size { get; }

        /// <summary>
        /// The children area's top left corner's position relative to the node's top left corner.
        /// </summary>
        Point2D ChildrenAreaTopLeft { get; }

        Size2D ChildrenAreaSize { get; }

        /// <summary>
        /// This node's level in the parent-child hierarchy.
        /// </summary>
        int HierarchyLevel { get; }

        [NotNull]
        IDiagramNode WithModelNode([NotNull] IModelNode newModelNode);

        [NotNull]
        IDiagramNode WithParentNode(ModelNodeId? newParentNodeId, int newHierarchyLevel);

        [NotNull]
        IDiagramNode WithAbsoluteTopLeft(Point2D newAbsoluteTopLeft);

        [NotNull]
        IDiagramNode WithRelativeTopLeft(Point2D newRelativeTopLeft);

        [NotNull]
        IDiagramNode WithSize(Size2D newSize);

        [NotNull]
        IDiagramNode WithChildrenAreaTopLeft(Point2D newTopLeft);

        [NotNull]
        IDiagramNode WithChildrenAreaSize(Size2D newSize);
    }
}