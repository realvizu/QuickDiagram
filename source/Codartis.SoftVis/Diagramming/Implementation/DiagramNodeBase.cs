using System;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;

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
        public IContainerDiagramNode ParentDiagramNode { get; }

        protected DiagramNodeBase(IModelNode modelNode, IContainerDiagramNode parentDiagramNode)
            : this(modelNode, Size2D.Zero, Point2D.Undefined, parentDiagramNode)
        {
        }

        protected DiagramNodeBase(IModelNode modelNode, Size2D size, Point2D center, IContainerDiagramNode parentDiagramNode)
        {
            ModelNode = modelNode ?? throw new ArgumentNullException(nameof(modelNode));
            Size = size;
            Center = center;
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

        public IDiagramNode WithModelNode(IModelNode modelNode) => CreateInstance(modelNode, Size, Center);
        public IDiagramNode WithSize(Size2D newSize) => CreateInstance(ModelNode, newSize, Center);
        public IDiagramNode WithCenter(Point2D newCenter) => CreateInstance(ModelNode, Size, newCenter);

        protected abstract IDiagramNode CreateInstance(IModelNode modelNode, Size2D size, Point2D center);
    }
}