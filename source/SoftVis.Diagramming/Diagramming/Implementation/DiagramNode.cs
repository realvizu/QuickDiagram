using System;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Diagramming.Implementation
{
    /// <summary>
    /// An immutable implementation of a diagram node.
    /// </summary>
    public class DiagramNode : DiagramShapeBase, IDiagramNode
    {
        public IModelNode ModelNode { get; }
        public Size2D Size { get; }
        public Point2D Center { get; }

        public DiagramNode(IModelNode modelNode)
            : this(modelNode, Size2D.Zero, Point2D.Undefined)
        {
        }

        public DiagramNode(IModelNode modelNode, Size2D size, Point2D center)
        {
            ModelNode = modelNode ?? throw new ArgumentNullException(nameof(modelNode));
            Size = size;
            Center = center;
        }

        public override bool IsRectDefined => Size.IsDefined && Center.IsDefined;
        public override Rect2D Rect => Rect2D.CreateFromCenterAndSize(Center, Size);

        public ModelNodeId Id => ModelNode.Id;
        public string Name => ModelNode.Name;
        public ModelNodeStereotype Stereotype => ModelNode.Stereotype;
        public ModelOrigin Origin => ModelNode.Origin;

        public Point2D TopLeft => Rect.TopLeft;
        public double Width => Size.Width;
        public double Height => Size.Height;

        public int CompareTo(IDiagramNode otherNode) =>
            string.Compare(Name, otherNode.Name, StringComparison.InvariantCultureIgnoreCase);

        public override string ToString() => Name;

        public IDiagramNode WithModelNode(IModelNode modelNode) => CreateInstance(modelNode, Size, Center);
        public IDiagramNode WithSize(Size2D newSize)  => CreateInstance(ModelNode, newSize, Center);
        public IDiagramNode WithCenter(Point2D newCenter) => CreateInstance(ModelNode, Size, newCenter);

        protected virtual DiagramNode CreateInstance(IModelNode modelNode, Size2D size, Point2D center) 
            => new DiagramNode(modelNode, size, center);
    }
}
