namespace Codartis.SoftVis.Diagramming.Layout.Incremental.Relative
{
    /// <summary>
    /// The implementer performs some action for relative layout changes.
    /// </summary>
    internal interface IRelativeLayoutChangeConsumer
    {
        void OnLayoutCleared();
        void OnVertexAdded(LayoutVertexBase vertex, RelativeLocation newLocation, ILayoutAction causingAction);
        void OnVertexRemoved(LayoutVertexBase vertex, RelativeLocation oldLocation, ILayoutAction causingAction);
        void OnVertexMoved(LayoutVertexBase vertex, RelativeLocation oldLocation, RelativeLocation newLocation, ILayoutAction causingAction);
        void OnPathAdded(LayoutPath layoutPath, ILayoutAction causingAction);
        void OnPathRemoved(LayoutPath layoutPath, ILayoutAction causingAction);
    }
}
