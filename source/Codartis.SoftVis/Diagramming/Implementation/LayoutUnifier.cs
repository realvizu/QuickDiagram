using System.Collections.Generic;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Geometry;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Implementation
{
    /// <summary>
    /// Unifies a hierarchy of layout groups (containing relative layout info)
    /// by calculating the absolute positions of all nodes (as implied by the hierarchy).
    /// </summary>
    public sealed class LayoutUnifier
    {
        private readonly Point2D _childrenAreaPadding;

        public LayoutUnifier(double childrenAreaPadding)
        {
            _childrenAreaPadding = new Point2D(childrenAreaPadding, childrenAreaPadding);
        }

        [NotNull]
        public GroupLayoutInfo CalculateAbsoluteLayout([NotNull] GroupLayoutInfo root)
        {
            return CalculateAbsoluteLayoutRecursive(
                root,
                Rect2D.Zero,
                Size2D.Zero,
                Rect2D.Zero,
                Point2D.Zero,
                _childrenAreaPadding);
        }

        [NotNull]
        private static GroupLayoutInfo CalculateAbsoluteLayoutRecursive(
            [NotNull] GroupLayoutInfo groupLayoutInfo,
            Rect2D parentRect,
            Size2D parentPayloadAreaSize,
            Rect2D parentChildrenAreaRect,
            Point2D padding,
            Point2D childrenAreaPadding)
        {
            var newNodeLayout = new List<BoxLayoutInfo>();

            foreach (var boxLayoutInfo in groupLayoutInfo.Boxes)
            {
                var absoluteBoxLayoutInfo = new BoxLayoutInfo(
                    boxLayoutInfo.BoxShape,
                    parentRect.TopLeft + new Point2D(0, parentPayloadAreaSize.Height) + padding + boxLayoutInfo.Rect.TopLeft - parentChildrenAreaRect.TopLeft);

                if (boxLayoutInfo.ChildrenArea != null)
                    absoluteBoxLayoutInfo.ChildrenArea = CalculateAbsoluteLayoutRecursive(
                        boxLayoutInfo.ChildrenArea,
                        boxLayoutInfo.Rect,
                        boxLayoutInfo.BoxShape.PayloadAreaSize,
                        boxLayoutInfo.ChildrenArea.Rect,
                        childrenAreaPadding,
                        childrenAreaPadding);

                newNodeLayout.Add(absoluteBoxLayoutInfo);
            }

            return new GroupLayoutInfo(newNodeLayout, groupLayoutInfo.Lines);
        }
    }
}