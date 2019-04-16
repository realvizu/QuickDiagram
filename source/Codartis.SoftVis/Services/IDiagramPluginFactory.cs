namespace Codartis.SoftVis.Services
{
    /// <summary>
    /// Creates diagram plugin instances.
    /// </summary>
    public interface IDiagramPluginFactory
    {
        IDiagramPlugin Create(DiagramPluginId diagramPluginId);
    }
}
