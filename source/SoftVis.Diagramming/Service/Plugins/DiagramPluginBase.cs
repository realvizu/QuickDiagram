using System;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Service.Plugins
{
    /// <summary>
    /// Abstract base class for diagram store plugins.
    /// </summary>
    public abstract class DiagramPluginBase : IDiagramPlugin, IDisposable
    {
        protected IReadOnlyModelStore ModelStore { get; private set; }
        protected IDiagramStore DiagramStore { get; private set; }

        public virtual void Initialize(IReadOnlyModelStore modelStore, IDiagramStore diagramStore)
        {
            ModelStore = modelStore ?? throw new ArgumentNullException(nameof(modelStore));
            DiagramStore = diagramStore ?? throw new ArgumentNullException(nameof(diagramStore));
        }

        public virtual void Dispose()
        {
        }
    }
}
