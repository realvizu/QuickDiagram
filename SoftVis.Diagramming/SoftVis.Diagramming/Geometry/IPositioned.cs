namespace Codartis.SoftVis.Geometry
{
    /// <summary>
    /// Something that has a location (position) in 2D space.
    /// </summary>
    public interface IPositioned
    {
        Point2D Center { get; }
    }
}
