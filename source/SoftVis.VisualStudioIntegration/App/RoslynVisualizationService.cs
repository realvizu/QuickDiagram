using System.Collections.Generic;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Service;
using Codartis.SoftVis.UI;
using Codartis.SoftVis.VisualStudioIntegration.Diagramming;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;
using Codartis.SoftVis.VisualStudioIntegration.UI;

namespace Codartis.SoftVis.VisualStudioIntegration.App
{
    /// <summary>
    /// Creates, aggregates and orchestrates model, diagram and UI services for Roslyn-based models.
    /// </summary>
    internal class RoslynVisualizationService : VisualizationService, IRoslynVisualizationService
    {
        public RoslynVisualizationService(
            IModelServiceFactory modelServiceFactory, 
            IDiagramServiceFactory diagramServiceFactory, 
            IUiServiceFactory uiServiceFactory, 
            IDiagramPluginFactory diagramPluginFactory, 
            IEnumerable<DiagramPluginId> diagramPluginIds) 
            : base(modelServiceFactory, 
                  diagramServiceFactory, 
                  uiServiceFactory, 
                  diagramPluginFactory, 
                  diagramPluginIds)
        {
        }

        public IRoslynModelService GetRoslynModelService() 
            => (IRoslynModelService) GetModelService();

        public IRoslynDiagramService GetRoslynDiagramService(DiagramId diagramId) 
            => (IRoslynDiagramService) GetDiagramService(diagramId);

        public IRoslynUiService GetRoslynUiService(DiagramId diagramId)
            => (IRoslynUiService)GetUiService(diagramId);
    }
}
