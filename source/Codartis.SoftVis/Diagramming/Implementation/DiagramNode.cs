using System;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.Util;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Implementation
{
    /// <summary>
    /// A diagram node.
    /// Immutable.
    /// </summary>
    public sealed class DiagramNode : DiagramShapeBase, IDiagramNode
    {
        public override string ShapeId { get; }
        public override Rect2D AbsoluteRect { get; }
        public override Rect2D RelativeRect { get; }

        public IModelNode ModelNode { get; }
        public Maybe<ModelNodeId> ParentNodeId { get; }
        public DateTime AddedAt { get; }
        public Point2D AbsoluteTopLeft { get; }
        public Point2D RelativeTopLeft { get; }
        public Size2D HeaderSize { get; }
        public Size2D ChildrenAreaSize { get; }
        public Point2D ChildrenAreaTopLeft { get; }
        public Size2D Size { get; }

        public DiagramNode(
            [NotNull] IModelNode modelNode,
            DateTime addedAt,
            Point2D absoluteTopLeft,
            Point2D relativeTopLeft,
            Size2D headerSize,
            Size2D childrenAreaSize,
            Point2D childrenAreaTopLeft,
            Maybe<ModelNodeId> parentNodeId)
        {
            ModelNode = modelNode;
            AbsoluteTopLeft = absoluteTopLeft;
            RelativeTopLeft = relativeTopLeft;
            AddedAt = addedAt;
            HeaderSize = headerSize;
            ChildrenAreaSize = childrenAreaSize;
            ChildrenAreaTopLeft = childrenAreaTopLeft;
            Size = CalculateSize();
            ParentNodeId = parentNodeId;

            ShapeId = modelNode.Id.ToShapeId();
            AbsoluteRect = CalculateAbsoluteRect();
            RelativeRect = CalculateRelativeRect();
        }

        public DiagramNode([NotNull] IModelNode modelNode)
            : this(
                modelNode,
                DateTime.Now,
                Point2D.Undefined,
                Point2D.Undefined,
                Size2D.Zero,
                Size2D.Zero,
                Point2D.Zero,
                Maybe<ModelNodeId>.Nothing)
        {
        }

        public bool HasParent => ParentNodeId.HasValue;
        public ModelNodeId Id => ModelNode.Id;
        public string Name => ModelNode.Name;
        public Point2D Center => AbsoluteRect.Center;

        public double Width => Size.Width;
        public double Height => Size.Height;

        public int CompareTo(IDiagramNode otherNode) => string.Compare(Name, otherNode.Name, StringComparison.InvariantCultureIgnoreCase);

        public override string ToString() => ModelNode.ToString();

        public IDiagramNode WithModelNode(IModelNode newModelNode)
            => CreateInstance(newModelNode, AddedAt, AbsoluteTopLeft, RelativeTopLeft, HeaderSize, ChildrenAreaSize, ChildrenAreaTopLeft, ParentNodeId);

        public IDiagramNode WithParentNodeId(ModelNodeId? newParentNodeId)
            => CreateInstance(
                ModelNode,
                AddedAt,
                AbsoluteTopLeft,
                RelativeTopLeft,
                HeaderSize,
                ChildrenAreaSize,
                ChildrenAreaTopLeft,
                newParentNodeId.ToMaybe());

        public IDiagramNode WithAbsoluteTopLeft(Point2D newAbsoluteTopLeft)
            => CreateInstance(ModelNode, AddedAt, newAbsoluteTopLeft, RelativeTopLeft, HeaderSize, ChildrenAreaSize, ChildrenAreaTopLeft, ParentNodeId);

        public IDiagramNode WithRelativeTopLeft(Point2D newRelativeTopLeft)
            => CreateInstance(ModelNode, AddedAt, AbsoluteTopLeft, newRelativeTopLeft, HeaderSize, ChildrenAreaSize, ChildrenAreaTopLeft, ParentNodeId);

        public IDiagramNode WithHeaderSize(Size2D newSize)
            => CreateInstance(ModelNode, AddedAt, AbsoluteTopLeft, RelativeTopLeft, newSize, ChildrenAreaSize, ChildrenAreaTopLeft, ParentNodeId);

        public IDiagramNode WithChildrenAreaSize(Size2D newSize)
            => CreateInstance(ModelNode, AddedAt, AbsoluteTopLeft, RelativeTopLeft, HeaderSize, newSize, ChildrenAreaTopLeft, ParentNodeId);

        public IDiagramNode WithChildrenAreaTopLeft(Point2D newTopLeft)
            => CreateInstance(ModelNode, AddedAt, AbsoluteTopLeft, RelativeTopLeft, HeaderSize, ChildrenAreaSize, newTopLeft, ParentNodeId);

        private Size2D CalculateSize() => new Size2D(Math.Max(HeaderSize.Width, ChildrenAreaSize.Width), HeaderSize.Height + ChildrenAreaSize.Height);

        private Rect2D CalculateAbsoluteRect() => new Rect2D(AbsoluteTopLeft, Size);

        private Rect2D CalculateRelativeRect() => new Rect2D(RelativeTopLeft, Size);

        [NotNull]
        private static IDiagramNode CreateInstance(
            [NotNull] IModelNode modelNode,
            DateTime addedAt,
            Point2D absoluteTopLeft,
            Point2D relativeTopLeft,
            Size2D headerSize,
            Size2D childrenAreaSize,
            Point2D childrenAreaTopLeft,
            Maybe<ModelNodeId> parentNodeId)
        {
            return new DiagramNode(
                modelNode,
                addedAt,
                absoluteTopLeft,
                relativeTopLeft,
                headerSize,
                childrenAreaSize,
                childrenAreaTopLeft,
                parentNodeId);
        }
    }
}