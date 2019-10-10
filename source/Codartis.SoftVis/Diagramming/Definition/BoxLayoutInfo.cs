using Codartis.SoftVis.Geometry;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Definition
{
    public sealed class BoxLayoutInfo : ILayoutInfo
    {
        private readonly Point2D _topLeft;
        [NotNull] public IBoxShape BoxShape { get; }
        [CanBeNull] public GroupLayoutInfo ChildrenArea { get; set; }

        public BoxLayoutInfo([NotNull] IBoxShape boxShape, Point2D topLeft, GroupLayoutInfo childrenArea = null)
        {
            BoxShape = boxShape;
            _topLeft = topLeft;
            ChildrenArea = childrenArea;
        }

        public Rect2D Rect => new Rect2D(_topLeft, CalculateSize(BoxShape, ChildrenArea));

        private static Size2D CalculateSize([NotNull] IBoxShape boxShape, [CanBeNull] GroupLayoutInfo childrenArea)
        {
            return Size2D.StackVertically(boxShape.PayloadAreaSize, childrenArea?.Rect.Size ?? Size2D.Zero);
        }
    }
}