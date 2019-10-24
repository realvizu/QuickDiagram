using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Geometry;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Definition
{
    public sealed class GroupLayoutInfo : ILayoutInfo
    {
        [NotNull] public IEnumerable<BoxLayoutInfo> Boxes { get; }
        [NotNull] public IEnumerable<LineLayoutInfo> Lines { get; }

        public GroupLayoutInfo(
            IEnumerable<BoxLayoutInfo> boxes = null,
            IEnumerable<LineLayoutInfo> lines = null)
        {
            Boxes = boxes ?? Enumerable.Empty<BoxLayoutInfo>();
            Lines = lines ?? Enumerable.Empty<LineLayoutInfo>();
        }

        public Rect2D Rect => CalculateRect();

        private Rect2D CalculateRect() => Boxes.Select(i => i.Rect).Concat(Lines.Select(i => i.Rect)).Union();

        [NotNull]
        public GroupLayoutInfo AddLineLayoutInfo([NotNull] IEnumerable<LineLayoutInfo> lines) => new GroupLayoutInfo(Boxes, Lines.Concat(lines));

        [CanBeNull]
        public BoxLayoutInfo GetBoxById([NotNull] string shapeId)
        {
            foreach (var box in Boxes)
            {
                if (box.ShapeId == shapeId)
                    return box;

                var childBox = box.ChildGroup?.GetBoxById(shapeId);
                if (childBox != null)
                    return childBox;
            }

            return null;
        }
    }
}