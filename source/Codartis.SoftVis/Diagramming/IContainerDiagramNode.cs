namespace Codartis.SoftVis.Diagramming
{
    /// <summary>
    /// A container node is a diagram node that can have child nodes that form a layout group.
    /// </summary>
    public interface IContainerDiagramNode : IDiagramNode
    {
        ILayoutGroup LayoutGroup { get; }

        IContainerDiagramNode WithLayoutGroup(ILayoutGroup layoutGroup);
    }
}
