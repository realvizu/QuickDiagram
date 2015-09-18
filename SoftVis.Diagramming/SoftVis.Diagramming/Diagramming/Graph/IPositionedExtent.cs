namespace Codartis.SoftVis.Diagramming.Graph
{
    public interface IPositionedExtent : IPositioned, IExtent
    {
        DiagramRect Rect { get; }
    }
}
