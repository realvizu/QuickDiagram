using Codartis.SoftVis.Diagramming;
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
        }

        private void SubscribeToDiagramEvents(IDiagramServices diagramServices)
        {
            diagramServices.ShapeActivated += OnDiagramShapeActivated;
            diagramServices.ShapeAdded += OnShapeAddedToDiagram;
        }

        /// <summary>
        /// When a shape is activated (i.e. double-clicked) opens the corresponding source file.
        /// </summary>
        /// <param name="diagramShape"></param>
        private void OnDiagramShapeActivated(IDiagramShape diagramShape)
        {
            var diagramNode = diagramShape as IDiagramNode;
            if (diagramNode == null)
                return;

            new ShowSourceFileCommand(this).Execute(diagramNode);
        }

        /// <summary>
        /// Whenever a shape is added to the diagram tries to expand the model with the related entities.
        /// </summary>
        /// <param name="diagramShape">The shape that was added to the diagram.</param>
        private void OnShapeAddedToDiagram(IDiagramShape diagramShape)
        {
            var diagramNode = diagramShape as IDiagramNode;
            if (diagramNode == null)
                return;

            ModelServices.ExtendModelWithRelatedEntities(diagramNode.ModelEntity);
        }
    }
}