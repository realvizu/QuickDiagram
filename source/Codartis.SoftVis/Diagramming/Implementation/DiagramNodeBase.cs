using System;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Diagramming.Implementation
{
    /// <summary>
    /// Abstract base for diagram nodes.
    /// </summary>
    [Immutable]
    public abstract class DiagramNodeBase : DiagramShapeBase, IDiagramNode
    {
        public IModelNode ModelNode { get; }
        public Size2D Size { get; }
        public Point2D Center { get; }
        public DateTime AddedAt { get; }
        public IContainerDiagramNode ParentDiagramNode { get; }

        protected DiagramNodeBase(IModelNode modelNode, IContainerDiagramNode parentDiagramNode)
            : this(modelNode, Size2D.Zero, Point2D.Undefined, DateTime.Now, parentDiagramNode)
        {
        }

        protected DiagramNodeBase(IModelNode modelNode, Size2D size, Point2D center, DateTime addedAt, IContainerDiagramNode parentDiagramNode)
        {
            ModelNode = modelNode ?? throw new ArgumentNullException(nameof(modelNode));
            Size = size;
            Center = center;
            AddedAt = addedAt;
            ParentDiagramNode = parentDiagramNode;
        }

        public override bool IsRectDefined => Size.IsDefined && Center.IsDefined;
        public override Rect2D Rect => Rect2D.CreateFromCenterAndSize(Center, Size);
        public bool HasParent => ParentDiagramNode != null;

        public ModelNodeId Id => ModelNode.Id;
        public string Name => ModelNode.Name;
        public ModelNodeStereotype Stereotype => ModelNode.Stereotype;
        public ModelOrigin Origin => ModelNode.Origin;

        public Point2D TopLeft => Rect.TopLeft;
        public double Width => Size.Width;
        public double Height => Size.Height;

        public int CompareTo(IDiagramNode otherNode)
            => string.Compare(Name, otherNode.Name, StringComparison.InvariantCultureIgnoreCase);

        public override string ToString() => Name;

        public IDiagramNode WithModelNode(IModelNode modelNode) => CreateInstance(modelNode, Size, Center, AddedAt, ParentDiagramNode);
        public IDiagramNode WithSize(Size2D newSize) => CreateInstance(ModelNode, newSize, Center, AddedAt, ParentDiagramNode);
        public IDiagramNode WithCenter(Point2D newCenter) => CreateInstance(ModelNode, Size, newCenter, AddedAt, ParentDiagramNode);
        public IDiagramNode WithTopLeft(Point2D newTopLeft) => CreateInstance(ModelNode, Size, newTopLeft + Size / 2, AddedAt, ParentDiagramNode);

        protected abstract IDiagramNode CreateInstance(IModelNode modelNode, Size2D size, Point2D center, DateTime addedAt, IContainerDiagramNode parentDiagramNode);
    }
}