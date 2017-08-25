namespace Codartis.SoftVis.Diagramming
{
    /// <summary>
    /// Creates diagram store instances.
    /// </summary>
    public interface IDiagramStoreFactory
    {
        IDiagramStore Create();
    }
}
