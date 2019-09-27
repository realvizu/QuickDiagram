using Codartis.SoftVis.Geometry;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Definition
{
    public sealed class NodeLayoutInfo : ILayoutInfo
    {
        [NotNull] public IDiagramNode Node { get; }
        [CanBeNull] public GroupLayoutInfo ChildrenArea { get; set; }
        public Rect2D Rect { get; }

        public NodeLayoutInfo([NotNull] IDiagramNode node, Point2D topLeft, [CanBeNull] GroupLayoutInfo childrenArea = null)
        {
            Node = node;
            ChildrenArea = childrenArea;
            Rect = new Rect2D(topLeft, CalculateSize(Node, ChildrenArea));
        }

        private static Size2D CalculateSize([NotNull] IDiagramNode diagramNode, [CanBeNull] GroupLayoutInfo childrenArea)
        {
            return Size2D.StackVertically(diagramNode.PayloadAreaSize, childrenArea?.Rect.Size ?? Size2D.Zero);
        }
    }
}