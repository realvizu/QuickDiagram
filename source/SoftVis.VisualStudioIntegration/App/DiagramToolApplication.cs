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
        public IModelServices ModelServices { get; }
        public IDiagramServices DiagramServices { get; }
        public IUiServices UiServices { get; }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public DiagramToolApplication(IModelServices modelServices, IDiagramServices diagramServices, IUiServices uiServices)
        {
            ModelServices = modelServices;
            DiagramServices = diagramServices;
            UiServices = uiServices;

            UiServices.ImageExportDpi = Dpi.Dpi150;

            SubscribeToDiagramEvents(diagramServices);
            SubscribeToUiEvents(uiServices);
        }

        private void SubscribeToDiagramEvents(IDiagramServices diagramServices)
        {
            diagramServices.ShapeAdded += OnShapeAdded;
        }

        private void SubscribeToUiEvents(IUiServices uiServices)
        {
            uiServices.ShowSourceRequested += OnShowSourceRequested;
            uiServices.ShowModelItemsRequested += OnShowItemsRequestedAsync;
        }

        /// <summary>
        /// Whenever a shape is added to the diagram we try to proactively expand the model with the related entities.
        /// </summary>
        /// <param name="diagramShape">The shape that was added to the diagram.</param>
        private void OnShapeAdded(IDiagramShape diagramShape)
        {
            var diagramNode = diagramShape as IDiagramNode;
            if (diagramNode == null)
                return;

            ModelServices.ExtendModelWithRelatedEntities(diagramNode.ModelNode);
        }

        private void OnShowSourceRequested(IDiagramShape diagramShape)
        {
            var diagramNode = diagramShape as IDiagramNode;
            if (diagramNode == null)
                return;

            new ShowSourceFileCommand(this).Execute(diagramNode);
        }

        private async void OnShowItemsRequestedAsync(IReadOnlyList<IModelNode> modelNodes)
        {
            var roslynModelNodes = modelNodes.OfType<IRoslynModelNode>().ToArray();

            if (roslynModelNodes.Any())
                await new AddItemsToDiagramCommand(this).ExecuteAsync(roslynModelNodes);
        }
    }
}