using System;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling.Definition;

namespace Codartis.SoftVis.Diagramming.Implementation
{
    /// <summary>
    /// Abstract base for diagram nodes. Immutable.
    /// </summary>
    public abstract class DiagramNodeBase : DiagramShapeBase, IDiagramNode
    {
        public IModelNode ModelNode { get; }
        public Size2D Size { get; }
        public Point2D Center { get; }
        public DateTime AddedAt { get; }
        public ModelNodeId? ParentNodeId { get; }

        protected DiagramNodeBase(IModelNode modelNode, ModelNodeId? parentNodeId = null)
            : this(modelNode, Size2D.Zero, Point2D.Undefined, DateTime.Now, parentNodeId)
        {
        }

        protected DiagramNodeBase(IModelNode modelNode, Size2D size, Point2D center, DateTime addedAt, ModelNodeId? parentNodeId)
        {
            ModelNode = modelNode ?? throw new ArgumentNullException(nameof(modelNode));
            Size = size;
            Center = center;
            AddedAt = addedAt;
            ParentNodeId = parentNodeId;
        }

        public override bool IsRectDefined => Size.IsDefined && Center.IsDefined;
        public override Rect2D Rect => Rect2D.CreateFromCenterAndSize(Center, Size);
        public bool HasParent => ParentNodeId != null;

        public ModelNodeId Id => ModelNode.Id;
        public string Name => ModelNode.Name;
        public ModelNodeStereotype Stereotype => ModelNode.Stereotype;
        public ModelOrigin Origin => ModelNode.Origin;

        public Point2D TopLeft => Rect.TopLeft;
        public double Width => Size.Width;
        public double Height => Size.Height;

        public int CompareTo(IDiagramNode otherNode) => string.Compare(Name, otherNode.Name, StringComparison.InvariantCultureIgnoreCase);

        public override string ToString() => Name;

        public IDiagramNode WithModelNode(IModelNode modelNode) => CreateInstance(modelNode, Size, Center, AddedAt, ParentNodeId);
        public IDiagramNode WithSize(Size2D newSize) => CreateInstance(ModelNode, newSize, Center, AddedAt, ParentNodeId);
        public IDiagramNode WithCenter(Point2D newCenter) => CreateInstance(ModelNode, Size, newCenter, AddedAt, ParentNodeId);
        public IDiagramNode WithTopLeft(Point2D newTopLeft) => CreateInstance(ModelNode, Size, newTopLeft + Size / 2, AddedAt, ParentNodeId);

        public IDiagramNode WithParentNodeId(ModelNodeId? newParentNodeId)
            => CreateInstance(ModelNode, Size, Center, AddedAt, newParentNodeId);

        protected abstract IDiagramNode CreateInstance(
            IModelNode modelNode,
            Size2D size,
            Point2D center,
            DateTime addedAt,
            ModelNodeId? parentNodeId);
    }
}