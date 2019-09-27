using Codartis.SoftVis.Geometry;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Definition
{
    public sealed class NodeLayoutInfo : ILayoutInfo
    {
        private readonly Point2D _topLeft;
        [NotNull] public IDiagramNode Node { get; }
        [CanBeNull] public GroupLayoutInfo ChildrenArea { get; set; }

        public NodeLayoutInfo([NotNull] IDiagramNode node, Point2D topLeft)
        {
            Node = node;
            _topLeft = topLeft;
        }

        public Rect2D Rect => new Rect2D(_topLeft, CalculateSize(Node, ChildrenArea));

        private static Size2D CalculateSize([NotNull] IDiagramNode diagramNode, [CanBeNull] GroupLayoutInfo childrenArea)
        {
            return Size2D.StackVertically(diagramNode.PayloadAreaSize, childrenArea?.Rect.Size ?? Size2D.Zero);
        }
    }
}