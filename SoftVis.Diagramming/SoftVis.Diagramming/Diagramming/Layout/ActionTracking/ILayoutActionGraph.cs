using QuickGraph;

namespace Codartis.SoftVis.Diagramming.Layout.ActionTracking
{
    /// <summary>
    /// Represents the actions of a layout logic run and the causality relationship between actions.
    /// </summary>
    public interface ILayoutActionGraph : IBidirectionalGraph<ILayoutAction, ILayoutActionEdge>
    {
        int VertexMoveCount { get; }

        string Serialize();
        void Save(string filename);
    }
}
