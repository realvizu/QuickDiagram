namespace Codartis.SoftVis.Geometry
{
    /// <summary>
    /// Something that has size in 2D space.
    /// </summary>
    public interface ISized
    {
        double Width { get; }
        double Height { get; }
        Size2D Size { get; }
    }
}
