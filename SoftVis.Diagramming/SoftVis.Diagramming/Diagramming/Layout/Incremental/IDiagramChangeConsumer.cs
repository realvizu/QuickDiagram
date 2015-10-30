namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// The implementor performs some action for each diagram change.
    /// </summary>
    internal interface IDiagramChangeConsumer
    {
        void Clear();
        void Add(DiagramNode diagramNode);
        void Remove(DiagramNode diagramNode);
        void Add(DiagramConnector diagramConnector);
        void Remove(DiagramConnector diagramConnector);
    }
}
