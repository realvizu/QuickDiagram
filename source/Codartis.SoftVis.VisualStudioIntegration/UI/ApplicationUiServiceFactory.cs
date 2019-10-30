using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.UI;

namespace Codartis.SoftVis.VisualStudioIntegration.UI
{
    /// <summary>
    /// Creates UI service instances for roslyn-based diagrams.
    /// </summary>
    internal class ApplicationUiServiceFactory : IUiServiceFactory
    {
        private readonly IHostUiServices _hostUiServices;
        private readonly bool _initialIsDescriptionVisible;

        public ApplicationUiServiceFactory(IHostUiServices hostUiServices, bool initialIsDescriptionVisible)
        {
            _hostUiServices = hostUiServices;
            _initialIsDescriptionVisible = initialIsDescriptionVisible;
        }

        public IUiService Create(
            IModelService modelService,
            IDiagramService diagramService,
            IRelatedNodeTypeProvider relatedNodeTypeProvider,
            double minZoom,
            double maxZoom,
            double initialZoom)
        {
            var diagramViewModel = new RoslynDiagramViewModel(
                modelService,
                diagramService,
                relatedNodeTypeProvider,
                _initialIsDescriptionVisible,
                minZoom,
                maxZoom,
                initialZoom);

            return new ApplicationUiService(_hostUiServices, diagramViewModel);
        }
    }
}