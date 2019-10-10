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
        public Rect2D Rect { get; }

        public GroupLayoutInfo(
            IEnumerable<BoxLayoutInfo> boxes = null,
            IEnumerable<LineLayoutInfo> lines = null)
        {
            Boxes = boxes ?? Enumerable.Empty<BoxLayoutInfo>();
            Lines = lines ?? Enumerable.Empty<LineLayoutInfo>();
            Rect = Boxes.Select(i => i.Rect).Concat(Lines.Select(i => i.Rect)).Union();
        }
    }
}