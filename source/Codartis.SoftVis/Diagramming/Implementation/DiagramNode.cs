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
        public override Rect2D Rect { get; }
        public IModelNode ModelNode { get; }
        public DateTime AddedAt { get; }
        public Maybe<ModelNodeId> ParentNodeId { get; }

        public DiagramNode(
            [NotNull] IModelNode modelNode,
            Rect2D rect,
            DateTime addedAt,
            Maybe<ModelNodeId> parentNodeId)
        {
            ModelNode = modelNode;
            Rect = rect;
            AddedAt = addedAt;
            ParentNodeId = parentNodeId;
        }

        public DiagramNode([NotNull] IModelNode modelNode)
            : this(
                modelNode,
                Rect2D.Undefined,
                DateTime.Now,
                Maybe<ModelNodeId>.Nothing)
        {
        }

        public bool HasParent => ParentNodeId.HasValue;
        public ModelNodeId Id => ModelNode.Id;
        public string Name => ModelNode.Name;
        public ModelNodeStereotype Stereotype => ModelNode.Stereotype;

        public Size2D Size => Rect.Size;
        public Point2D Center => Rect.Center;
        public Point2D TopLeft => Rect.TopLeft;
        public double Width => Rect.Size.Width;
        public double Height => Rect.Size.Height;

        public int CompareTo(IDiagramNode otherNode) => string.Compare(Name, otherNode.Name, StringComparison.InvariantCultureIgnoreCase);

        public override string ToString() => ModelNode.ToString();

        public IDiagramNode WithModelNode(IModelNode newModelNode) => CreateInstance(newModelNode, Rect, AddedAt, ParentNodeId);
        public IDiagramNode WithParentNodeId(Maybe<ModelNodeId> newParentNodeId)=> CreateInstance(ModelNode, Rect, AddedAt, newParentNodeId);
        public IDiagramNode WithRect(Rect2D newRect) => CreateInstance(ModelNode, newRect, AddedAt, ParentNodeId);
        public IDiagramNode WithSize(Size2D newSize) => CreateInstance(ModelNode, Rect.WithSize(newSize), AddedAt, ParentNodeId);
        public IDiagramNode WithCenter(Point2D newCenter) => CreateInstance(ModelNode, Rect.WithCenter(newCenter), AddedAt, ParentNodeId);
        public IDiagramNode WithTopLeft(Point2D newTopLeft) => CreateInstance(ModelNode, Rect.WithTopLeft(newTopLeft), AddedAt, ParentNodeId);

        [NotNull]
        private static IDiagramNode CreateInstance([NotNull] IModelNode modelNode, Rect2D rect, DateTime addedAt, Maybe<ModelNodeId> parentNodeId)
            => new DiagramNode(modelNode, rect, addedAt, parentNodeId);
    }
}