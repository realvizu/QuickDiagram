namespace Codartis.SoftVis.UI.Geometry
{
    /// <summary>
    /// Alignment to a rectangle.
    /// </summary>
    public struct RectAlignment
    {
        public VerticalAlignmentType VerticalAlignment { get; }
        public HorizontalAlignmentType HorizontalAlignment { get; }
        
        private RectAlignment(VerticalAlignmentType verticalAlignment, HorizontalAlignmentType horizontalAlignment)
        {
            VerticalAlignment = verticalAlignment;
            HorizontalAlignment = horizontalAlignment;
        }

        public static RectAlignment TopLeft = new RectAlignment(VerticalAlignmentType.Top, HorizontalAlignmentType.Left);
        public static RectAlignment TopMiddle = new RectAlignment(VerticalAlignmentType.Top, HorizontalAlignmentType.Middle);
        public static RectAlignment TopRight = new RectAlignment(VerticalAlignmentType.Top, HorizontalAlignmentType.Right);

        public static RectAlignment MiddleLeft = new RectAlignment(VerticalAlignmentType.Middle, HorizontalAlignmentType.Left);
        public static RectAlignment Center = new RectAlignment(VerticalAlignmentType.Middle, HorizontalAlignmentType.Middle);
        public static RectAlignment MiddleRight = new RectAlignment(VerticalAlignmentType.Middle, HorizontalAlignmentType.Right);

        public static RectAlignment BottomLeft = new RectAlignment(VerticalAlignmentType.Bottom, HorizontalAlignmentType.Left);
        public static RectAlignment BottomMiddle = new RectAlignment(VerticalAlignmentType.Bottom, HorizontalAlignmentType.Middle);
        public static RectAlignment BottomRight = new RectAlignment(VerticalAlignmentType.Bottom, HorizontalAlignmentType.Right);
    }
}
