using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.UI;

namespace Codartis.SoftVis.Services
{
    /// <summary>
    /// Creates, aggregates and orchestrates model, diagram and UI services.
    /// Manages 1 model and any number of diagrams based on that model.
    /// Diagrams can have plugins that react to model and/or diagram events to perform useful tasks, eg. arrange the diagram.
    /// </summary>
    public class VisualizationService : IVisualizationService
    {
        private const double DefaultMinZoom = .1;
        private const double DefaultMaxZoom = 10;
        private const double DefaultInitialZoom = 1;

        protected readonly IModelServiceFactory ModelServiceFactory;
        protected readonly IDiagramServiceFactory DiagramServiceFactory;
        protected readonly IUiServiceFactory UiServiceFactory;
        protected readonly IDiagramPluginFactory DiagramPluginFactory;
        protected readonly IRelatedNodeTypeProvider RelatedNodeTypeProvider;
        protected readonly IEnumerable<DiagramPluginId> DiagramPluginIds;

        protected IModelService ModelService { get; }

        private readonly Dictionary<DiagramId, IDiagramService> _diagramServices;
        private readonly Dictionary<DiagramId, IUiService> _diagramUis;

        // This dictionary is just to root the plugins.
        // ReSharper disable once CollectionNeverQueried.Local
        private readonly Dictionary<DiagramId, List<IDiagramPlugin>> _diagramPlugins;

        public VisualizationService(
            IModelServiceFactory modelServiceFactory,
            IDiagramServiceFactory diagramServiceFactory,
            IUiServiceFactory uiServiceFactory,
            IDiagramPluginFactory diagramPluginFactory,
            IRelatedNodeTypeProvider relatedNodeTypeProvider,
            IEnumerable<DiagramPluginId> diagramPluginIds)
        {
            ModelServiceFactory = modelServiceFactory;
            DiagramServiceFactory = diagramServiceFactory;
            UiServiceFactory = uiServiceFactory;
            DiagramPluginFactory = diagramPluginFactory;
            RelatedNodeTypeProvider = relatedNodeTypeProvider;
            DiagramPluginIds = diagramPluginIds;

            ModelService = ModelServiceFactory.Create();
            _diagramServices = new Dictionary<DiagramId, IDiagramService>();
            _diagramUis = new Dictionary<DiagramId, IUiService>();
            _diagramPlugins = new Dictionary<DiagramId, List<IDiagramPlugin>>();
        }

        public IModelService GetModelService() => ModelService;

        public DiagramId CreateDiagram(
            double minZoom = DefaultMinZoom,
            double maxZoom = DefaultMaxZoom,
            double initialZoom = DefaultInitialZoom)
        {
            var diagramId = DiagramId.Create();
            var diagramService = DiagramServiceFactory.Create(GetModelService());
            _diagramServices.Add(diagramId, diagramService);

            var diagramUi = CreateDiagramUi(diagramId, minZoom, maxZoom, initialZoom);
            _diagramUis.Add(diagramId, diagramUi);

            // Warning: plugins must be created after the UI so its event callbacks don't precede UI updates.
            var diagramPlugins = CreateAndAttachDiagramPlugins(DiagramPluginIds, ModelService, diagramService);
            _diagramPlugins.Add(diagramId, diagramPlugins.ToList());

            return diagramId;
        }

        public IDiagramService GetDiagramService(DiagramId diagramId) => _diagramServices[diagramId];
        public IUiService GetUiService(DiagramId diagramId) => _diagramUis[diagramId];

        private IUiService CreateDiagramUi(DiagramId diagramId,
            double minZoom = DefaultMinZoom,
            double maxZoom = DefaultMaxZoom,
            double initialZoom = DefaultInitialZoom)
        {
            var diagramService = GetDiagramService(diagramId);
            var diagramUi = UiServiceFactory.Create(ModelService, diagramService, RelatedNodeTypeProvider, minZoom, maxZoom, initialZoom);

            diagramUi.DiagramNodePayloadAreaSizeChanged += (diagramNode, size) => OnDiagramNodePayloadAreaSizeChanged(diagramId, diagramNode, size);
            diagramUi.RemoveDiagramNodeRequested += diagramNode => OnRemoveDiagramNodeRequested(diagramId, diagramNode);

            return diagramUi;
        }

        private IEnumerable<IDiagramPlugin> CreateAndAttachDiagramPlugins(IEnumerable<DiagramPluginId> diagramPluginIds,
            IModelService modelService, IDiagramService diagramService)
        {
            foreach (var diagramPluginId in diagramPluginIds)
            {
                var diagramPlugin = DiagramPluginFactory.Create(diagramPluginId);
                diagramPlugin.Initialize(modelService, diagramService);
                yield return diagramPlugin;
            }
        }

        private void OnDiagramNodePayloadAreaSizeChanged(DiagramId diagramId, IDiagramNode diagramNode, Size2D newSize)
        {
            GetDiagramService(diagramId).UpdateNodePayloadAreaSize(diagramNode.Id, newSize);
        }

        private void OnRemoveDiagramNodeRequested(DiagramId diagramId, IDiagramNode diagramNode)
        {
            GetDiagramService(diagramId).RemoveNode(diagramNode.Id);
        }
    }
}
