using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;

namespace Codartis.SoftVis.Util.UI.Wpf
{
    /// <summary>
    /// Defines a position relative to a rectangle.
    /// </summary>
    public struct RectRelativePlacement
    {
        /// <summary>
        /// A translate vector with NaN values means the Undefined (extreme) value.
        /// </summary>
        public static readonly RectRelativePlacement Undefined =
            new RectRelativePlacement { Translate = new Vector(double.NaN, double.NaN) };

        public HorizontalAlignment Horizontal { get; set; }
        public VerticalAlignment Vertical { get; set; }
        public Vector Translate { get; set; }

        public RectRelativePlacement(HorizontalAlignment horizontal, VerticalAlignment vertical, Vector translate)
        {
            Horizontal = horizontal;
            Vertical = vertical;
            Translate = translate;
        }

        public bool Equals(RectRelativePlacement other)
        {
            return Horizontal == other.Horizontal
                && Vertical == other.Vertical
                && Translate.Equals(other.Translate);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is RectRelativePlacement && Equals((RectRelativePlacement)obj);
        }

        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)Horizontal;
                hashCode = (hashCode * 397) ^ (int)Vertical;
                hashCode = (hashCode * 397) ^ Translate.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(RectRelativePlacement left, RectRelativePlacement right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(RectRelativePlacement left, RectRelativePlacement right)
        {
            return !left.Equals(right);
        }

        public Point GetPositionRelativeTo(Rect rect)
        {
            return GetRelativePoint(rect, Horizontal, Vertical) + Translate;
        }

        private static Point GetRelativePoint(Rect rect, HorizontalAlignment horizontal, VerticalAlignment vertical)
        {
            return new Point(GetRelativeXCoordinate(rect, horizontal), GetRelativeYCoordinate(rect, vertical));
        }

        private static double GetRelativeXCoordinate(Rect rect, HorizontalAlignment horizontalAlignment)
        {
            switch (horizontalAlignment)
            {
                case HorizontalAlignment.Left: return rect.Left;
                case HorizontalAlignment.Center: return rect.Left + rect.Width / 2;
                case HorizontalAlignment.Right: return rect.Left + rect.Width;
                default: throw new ArgumentException($"Unexpected reference point: {horizontalAlignment}");
            }
        }

        private static double GetRelativeYCoordinate(Rect rect, VerticalAlignment verticalAlignment)
        {
            switch (verticalAlignment)
            {
                case VerticalAlignment.Top: return rect.Top;
                case VerticalAlignment.Center: return rect.Top + rect.Height / 2;
                case VerticalAlignment.Bottom: return rect.Top + rect.Height;
                default: throw new ArgumentException($"Unexpected reference point: {verticalAlignment}");
            }
        }
    }
}
