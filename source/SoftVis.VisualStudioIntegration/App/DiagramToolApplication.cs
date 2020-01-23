using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public sealed class DiagramToolApplication : IAppServices
    {
        public IModelServices ModelServices { get; }
        public IDiagramServices DiagramServices { get; }
        public IUiServices UiServices { get; }
        public IHostUiServices HostUiServices { get; }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public DiagramToolApplication(
            IModelServices modelServices,
            IDiagramServices diagramServices,
            IUiServices uiServices,
            IHostUiServices hostUiServices)
        {
            ModelServices = modelServices;
            DiagramServices = diagramServices;
            UiServices = uiServices;
            HostUiServices = hostUiServices;

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
            uiServices.ShowModelItemsRequested += OnShowItemsRequested;
        }

        private void OnShapeAdded(IDiagramShape diagramShape)
        {
            HostUiServices.RunAsync(async () => await OnShapeAddedAsync(diagramShape));
        }

        /// <summary>
        /// Whenever a shape is added to the diagram we try to proactively expand the model with the related entities.
        /// </summary>
        /// <param name="diagramShape">The shape that was added to the diagram.</param>
        private async Task OnShapeAddedAsync(IDiagramShape diagramShape)
        {
            var diagramNode = diagramShape as IDiagramNode;
            if (diagramNode == null)
                return;

            await ModelServices.ExtendModelWithRelatedEntitiesAsync(diagramNode.ModelEntity);
        }

        private void OnShowSourceRequested(IDiagramShape diagramShape)
        {
            HostUiServices.RunAsync(async () => await OnShowSourceRequestedAsync(diagramShape));
        }

        private async Task OnShowSourceRequestedAsync(IDiagramShape diagramShape)
        {
            var diagramNode = diagramShape as IDiagramNode;
            if (diagramNode == null)
                return;

            await new ShowSourceFileCommand(this).ExecuteAsync(diagramNode);
        }

        private void OnShowItemsRequested(IReadOnlyList<IModelEntity> modelEntities)
        {
            HostUiServices.RunAsync(async () => await OnShowItemsRequestAsync(modelEntities));
        }

        private async Task OnShowItemsRequestAsync(IReadOnlyList<IModelEntity> modelEntities)
        {
            if (!modelEntities.Any())
                return;

            await new AddItemsToDiagramCommand(this).ExecuteAsync(modelEntities);
        }
    }
}