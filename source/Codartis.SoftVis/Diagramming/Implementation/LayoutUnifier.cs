using System.Collections.Generic;
using System.Linq;
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

            var childTranslateVector = 
                parentTopLeft +
                new Point2D(0, parentPayloadAreaSize.Height) +
                new Point2D(padding, padding) +
                parentChildrenAreaRect.TopLeft;

            var absoluteBoxLayout = new List<BoxLayoutInfo>();

            foreach (var boxLayoutInfo in groupLayoutInfo.Boxes)
            {
                var boxTopLeft = boxLayoutInfo.TopLeft + childTranslateVector;

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

                absoluteBoxLayout.Add(absoluteBoxLayoutInfo);
            }

            var absoluteLineLayout = groupLayoutInfo.Lines.Select(i=>i.Translate(childTranslateVector));

            return new GroupLayoutInfo(absoluteBoxLayout, absoluteLineLayout);
        }
    }
}