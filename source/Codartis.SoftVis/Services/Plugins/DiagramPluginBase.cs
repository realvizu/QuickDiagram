using System;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Services.Plugins
{
    /// <summary>
    /// Abstract base class for diagram store plugins.
    /// </summary>
    public abstract class DiagramPluginBase : IDiagramPlugin, IDisposable
    {
        protected IModelService ModelService { get; private set; }
        protected IDiagramService DiagramService { get; private set; }

        public virtual void Initialize(IModelService modelService, IDiagramService diagramService)
        {
            ModelService = modelService ?? throw new ArgumentNullException(nameof(modelService));
            DiagramService = diagramService ?? throw new ArgumentNullException(nameof(diagramService));
        }

        public virtual void Dispose()
        {
        }
    }
}
