using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.VisualStudioIntegration.App.Commands;
using Codartis.SoftVis.VisualStudioIntegration.Diagramming;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;
using Codartis.SoftVis.VisualStudioIntegration.UI;

namespace Codartis.SoftVis.VisualStudioIntegration.App
{
    /// <summary>
    /// The diagram tool application.
    /// </summary>
    /// <remarks>
    /// Sets up the model, the diagram, and the commands that implement the application logic.
    /// Provides application services to the commands.
    /// </remarks>
    internal sealed class DiagramToolApplication : IAppServices
    {
        public IRoslynModelService RoslynModelService { get; }
        public IRoslynDiagramService RoslynDiagramService { get; }
        public IRoslynUiService RoslynUiService { get; }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public DiagramToolApplication(
            IRoslynModelService roslynModelService, 
            IRoslynDiagramService roslynDiagramService, 
            IRoslynUiService roslynUiService)
        {
            RoslynModelService = roslynModelService;
            RoslynDiagramService = roslynDiagramService;
            RoslynUiService = roslynUiService;

            RoslynUiService.ImageExportDpi = Dpi.Dpi150;

            //SubscribeToDiagramEvents(roslynDiagramService);
            //SubscribeToUiEvents(roslynUiService);
        }

        private void OnShowSourceRequested(IDiagramShape diagramShape)
        {
            var diagramNode = diagramShape as IDiagramNode;
            if (diagramNode == null)
                return;

            new ShowSourceFileCommand(this).Execute(diagramNode);
        }

        private async void OnShowItemsRequestedAsync(IReadOnlyList<IModelNode> modelNodes, bool followWithViewport)
        {
            var roslynModelNodes = modelNodes.OfType<IRoslynModelNode>().ToArray();

            if (roslynModelNodes.Any())
                await new AddItemsToDiagramCommand(this).ExecuteAsync(roslynModelNodes, followWithViewport);
        }
    }
}