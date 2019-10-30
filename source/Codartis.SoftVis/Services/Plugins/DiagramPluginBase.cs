using System;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Modeling.Definition;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Services.Plugins
{
    /// <summary>
    /// Abstract base class for diagram plugins.
    /// </summary>
    public abstract class DiagramPluginBase : IDiagramPlugin, IDisposable
    {
        [NotNull] protected IModelService ModelService { get; }
        [NotNull] protected IDiagramService DiagramService { get; }

        protected DiagramPluginBase(
            [NotNull] IModelService modelService,
            [NotNull] IDiagramService diagramService)
        {
            ModelService = modelService;
            DiagramService = diagramService;
        }

        public virtual void Dispose()
        {
        }
    }
}