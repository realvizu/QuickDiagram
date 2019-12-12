using Codartis.SoftVis.Diagramming.Definition;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Services.Plugins
{
    /// <summary>
    /// Abstract base class for diagram plugins.
    /// </summary>
    public abstract class DiagramPluginBase : IDiagramPlugin
    {
        [NotNull] protected IDiagramService DiagramService { get; }

        protected DiagramPluginBase([NotNull] IDiagramService diagramService)
        {
            DiagramService = diagramService;
        }

        public virtual void Dispose()
        {
        }
    }
}