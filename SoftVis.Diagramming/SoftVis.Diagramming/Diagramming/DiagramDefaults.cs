using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Diagramming
{
    /// <summary>
    /// Default values fro diagram layout.
    /// </summary>
    public static class DiagramDefaults
    {
        public const double MinimumNodeWidth = 40;
        public const double DefaultNodeWidth = 100;
        public const double MaximumNodeWidth = 250;
        public const double MinimumNodeHeight = 20;
        public const double DefaultNodeHeight = 38;
        public const double MaximumNodeHeight = 50;

        public const double HorizontalGap = 20;
        public const double VerticalGap = 40;

        public static readonly Point2D DefaultNodePosition = Point2D.Empty;
        public static readonly Size2D DefaultNodeSize = new Size2D(DefaultNodeWidth, DefaultNodeHeight);

        public static readonly Point2D DefaultLayoutStartingPoint = new Point2D(0, 0);
    }
}