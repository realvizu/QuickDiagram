namespace Codartis.SoftVis.Service
{
    /// <summary>
    /// Creates diagram plugin instances.
    /// </summary>
    public interface IDiagramPluginFactory
    {
        IDiagramPlugin Create(DiagramPluginId diagramPluginId);
    }
}
