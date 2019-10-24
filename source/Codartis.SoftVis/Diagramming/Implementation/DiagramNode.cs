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
        public IModelNode ModelNode { get; }
        public Maybe<ModelNodeId> ParentNodeId { get; }
        public DateTime AddedAt { get; }
        public Point2D TopLeft { get; }
        public Size2D PayloadAreaSize { get; }
        public Size2D ChildrenAreaSize { get; }

        public DiagramNode(
            [NotNull] IModelNode modelNode,
            DateTime addedAt,
            Point2D topLeft,
            Size2D payloadAreaSize,
            Size2D childrenAreaSize,
            Maybe<ModelNodeId> parentNodeId)
        {
            ModelNode = modelNode;
            TopLeft = topLeft;
            AddedAt = addedAt;
            PayloadAreaSize = payloadAreaSize;
            ChildrenAreaSize = childrenAreaSize;
            ParentNodeId = parentNodeId;
        }

        public DiagramNode([NotNull] IModelNode modelNode)
            : this(
                modelNode,
                DateTime.Now,
                Point2D.Undefined,
                Size2D.Zero,
                Size2D.Zero,
                Maybe<ModelNodeId>.Nothing)
        {
        }

        public override string ShapeId => ModelNode.Id.ToShapeId();
        public override Rect2D Rect => CalculateRect();
        public bool HasParent => ParentNodeId.HasValue;
        public ModelNodeId Id => ModelNode.Id;
        public string Name => ModelNode.Name;
        public Point2D Center => Rect.Center;
        public Size2D Size => Rect.Size;
        public double Width => Size.Width;
        public double Height => Size.Height;

        public int CompareTo(IDiagramNode otherNode) => string.Compare(Name, otherNode.Name, StringComparison.InvariantCultureIgnoreCase);

        public override string ToString() => ModelNode.ToString();

        public IDiagramNode WithModelNode(IModelNode newModelNode)
            => CreateInstance(newModelNode, AddedAt, TopLeft, PayloadAreaSize, ChildrenAreaSize, ParentNodeId);

        public IDiagramNode WithParentNodeId(ModelNodeId? newParentNodeId)
            => CreateInstance(ModelNode, AddedAt, TopLeft, PayloadAreaSize, ChildrenAreaSize, newParentNodeId.ToMaybe());

        public IDiagramNode WithCenter(Point2D newCenter)
            => CreateInstance(ModelNode, AddedAt, Rect.WithCenter(newCenter).TopLeft, PayloadAreaSize, ChildrenAreaSize, ParentNodeId);

        public IDiagramNode WithTopLeft(Point2D newTopLeft) => CreateInstance(ModelNode, AddedAt, newTopLeft, PayloadAreaSize, ChildrenAreaSize, ParentNodeId);
        public IDiagramNode WithPayloadAreaSize(Size2D newSize) => CreateInstance(ModelNode, AddedAt, TopLeft, newSize, ChildrenAreaSize, ParentNodeId);
        public IDiagramNode WithChildrenAreaSize(Size2D newSize) => CreateInstance(ModelNode, AddedAt, TopLeft, PayloadAreaSize, newSize, ParentNodeId);

        private Rect2D CalculateRect()
            => new Rect2D(TopLeft, new Size2D(Math.Max(PayloadAreaSize.Width, ChildrenAreaSize.Width), PayloadAreaSize.Height + ChildrenAreaSize.Height));

        [NotNull]
        private static IDiagramNode CreateInstance(
            [NotNull] IModelNode modelNode,
            DateTime addedAt,
            Point2D topLeft,
            Size2D payloadAreaSize,
            Size2D childrenAreaSize,
            Maybe<ModelNodeId> parentNodeId)
            => new DiagramNode(modelNode, addedAt, topLeft, payloadAreaSize, childrenAreaSize, parentNodeId);
    }
}