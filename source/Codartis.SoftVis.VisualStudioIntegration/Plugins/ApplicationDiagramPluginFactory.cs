using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Layout;
using Codartis.SoftVis.Services;
using Codartis.SoftVis.Services.Plugins;
using Codartis.SoftVis.VisualStudioIntegration.UI;

namespace Codartis.SoftVis.VisualStudioIntegration.Plugins
{
    /// <summary>
    /// Extends the diagram plugin factory with application-specific plugin creation logic.
    /// </summary>
    internal class ApplicationDiagramPluginFactory : DiagramPluginFactory
    {
        private readonly IHostUiServices _hostUiServices;

        public ApplicationDiagramPluginFactory(
            ILayoutPriorityProvider layoutPriorityProvider, 
            IDiagramShapeFactory diagramShapeFactory, 
            IHostUiServices hostUiServices)
            : base(layoutPriorityProvider, diagramShapeFactory)
        {
            _hostUiServices = hostUiServices; 
        }

        public override IDiagramPlugin Create(DiagramPluginId diagramPluginId)
        {
            if (diagramPluginId == ApplicationDiagramPluginId.ModelExtenderDiagramPlugin)
                return new ModelExtenderDiagramPlugin(_hostUiServices);

            return base.Create(diagramPluginId);
        }
    }
}