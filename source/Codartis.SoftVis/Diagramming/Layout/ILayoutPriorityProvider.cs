namespace Codartis.SoftVis.Diagramming.Layout
{
    /// <summary>
    /// Provides layout priority information about diagram nodes.
    /// </summary>
    public interface ILayoutPriorityProvider
    {
        int GetPriority(IDiagramNode diagramNode);
    }
}
