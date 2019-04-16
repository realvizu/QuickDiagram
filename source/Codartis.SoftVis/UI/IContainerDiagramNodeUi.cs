namespace Codartis.SoftVis.UI
{
    /// <summary>
    /// Abstraction for the UI of a diagram node that can contain other diagram nodes.
    /// </summary>
    public interface IContainerDiagramNodeUi: IDiagramNodeUi 
    {
        void AddChildNode(IDiagramNodeUi childNode);
    }
}