using System;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Modeling.Definition;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Services.Plugins
{
    /// <summary>
    /// Abstract base class for diagram store plugins.
    /// </summary>
    public abstract class DiagramPluginBase : IDiagramPlugin, IDisposable
    {
        protected IModelService ModelService { get; private set; }
        protected IDiagramService DiagramService { get; private set; }

        public virtual void Initialize([NotNull] IModelService modelService, [NotNull] IDiagramService diagramService)
        {
            ModelService = modelService;
            DiagramService = diagramService;
        }

        public virtual void Dispose()
        {
        }
    }
}