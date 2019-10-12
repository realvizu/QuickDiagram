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
        public double Padding { get; private set; }
        public Rect2D Rect { get; private set; }

        public GroupLayoutInfo(
            IEnumerable<BoxLayoutInfo> boxes = null,
            IEnumerable<LineLayoutInfo> lines = null,
            double padding = 0)
        {
            Boxes = boxes ?? Enumerable.Empty<BoxLayoutInfo>();
            Lines = lines ?? Enumerable.Empty<LineLayoutInfo>();
            SetPadding(padding);
        }

        public void SetPadding(double padding)
        {
            Padding = padding;
            Rect = CalculateRect();
        }

        private Rect2D CalculateRect()
        {
            var rect = Boxes.Select(i => i.Rect).Concat(Lines.Select(i => i.Rect)).Union();

            if (rect.IsDefined() && !rect.IsEmpty)
                rect = rect.WithMargin(Padding);

            return rect;
        }
    }
}