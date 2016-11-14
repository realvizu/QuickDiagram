namespace Codartis.SoftVis.Diagramming.Layout
{
    /// <summary>
    /// Consumes diagram actions.
    /// </summary>
    internal interface IDiagramActionConsumer
    {
        void AddDiagramNode(IDiagramNode diagramNode);
        void RemoveDiagramNode(IDiagramNode diagramNode);
        void ResizeDiagramNode(IDiagramNode diagramNode);
        void AddDiagramConnector(IDiagramConnector diagramConnector);
        void RemoveDiagramConnector(IDiagramConnector diagramConnector);
    }
}