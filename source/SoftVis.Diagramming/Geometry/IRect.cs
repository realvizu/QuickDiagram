namespace Codartis.SoftVis.Geometry
{
    /// <summary>
    /// A rectangular area in 2D space.
    /// </summary>
    /// <remarks>
    /// These rectangular areas can be compared to each other.
    /// Warning: the comparison is not necessarily based on location or size; it can be based on anything.
    /// </remarks>
    public interface IRect : IPositioned, ISized
    {
        Rect2D Rect { get; }        
    }
}
