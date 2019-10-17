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
        private readonly double _childrenAreaPadding;

        public LayoutUnifier(double childrenAreaPadding)
        {
            _childrenAreaPadding = childrenAreaPadding;
        }

        [NotNull]
        public GroupLayoutInfo CalculateAbsoluteLayout([NotNull] GroupLayoutInfo root)
        {
            return CalculateAbsoluteLayoutRecursive(
                root,
                Point2D.Zero, 
                Size2D.Zero,
                Rect2D.Zero,
                0);
        }

        private GroupLayoutInfo CalculateAbsoluteLayoutRecursive(
            GroupLayoutInfo groupLayoutInfo,
            Point2D parentTopLeft,
            Size2D parentPayloadAreaSize,
            Rect2D parentChildrenAreaRect,
            double padding)
        {
            if (groupLayoutInfo == null)
                return null;

            var newNodeLayout = new List<BoxLayoutInfo>();

            foreach (var boxLayoutInfo in groupLayoutInfo.Boxes)
            {
                var boxTopLeft =
                    parentTopLeft +
                    new Point2D(0, parentPayloadAreaSize.Height) +
                    new Point2D(padding, padding) +
                    boxLayoutInfo.TopLeft -
                    parentChildrenAreaRect.TopLeft;

                var childGroup = CalculateAbsoluteLayoutRecursive(
                    boxLayoutInfo.ChildGroup,
                    boxTopLeft,
                    boxLayoutInfo.PayloadAreaSize,
                    boxLayoutInfo.ChildGroup?.Rect ?? Rect2D.Undefined,
                    _childrenAreaPadding);

                var absoluteBoxLayoutInfo = new BoxLayoutInfo(
                        boxLayoutInfo.ShapeId,
                        boxTopLeft,
                        boxLayoutInfo.PayloadAreaSize,
                        childGroup?.Rect.Size.WithMargin(_childrenAreaPadding) ?? Size2D.Zero,
                        childGroup);

                newNodeLayout.Add(absoluteBoxLayoutInfo);
            }

            return new GroupLayoutInfo(newNodeLayout, groupLayoutInfo.Lines);
        }
    }
}