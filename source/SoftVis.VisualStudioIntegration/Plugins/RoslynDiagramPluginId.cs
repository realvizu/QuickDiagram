using Codartis.SoftVis.Service;

namespace Codartis.SoftVis.VisualStudioIntegration.Plugins
{
    /// <summary>
    /// Defines Roslyn-specific diagram plugin IDs.
    /// </summary>
    internal static class RoslynDiagramPluginId
    {
        public static readonly DiagramPluginId ModelExtenderDiagramPlugin = new DiagramPluginId(nameof(ModelExtenderDiagramPlugin));
    }
}
