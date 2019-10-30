using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.UI;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Services
{
    /// <summary>
    /// Creates, aggregates and orchestrates model, diagram and UI services.
    /// Manages 1 model and any number of diagrams based on that model.
    /// Diagrams can have plugins that react to model and/or diagram events to perform useful tasks, eg. arrange the diagram.
    /// </summary>
    public sealed class VisualizationService : IVisualizationService
    {
        private const double DefaultMinZoom = .1;
        private const double DefaultMaxZoom = 10;
        private const double DefaultInitialZoom = 1;

        [NotNull] private readonly IModelService _modelService;
        [NotNull] private readonly IDiagramServiceFactory _diagramServiceFactory;
        [NotNull] private readonly IUiServiceFactory _uiServiceFactory;
        [NotNull] private readonly IDiagramPluginFactory _diagramPluginFactory;
        [NotNull] private readonly IRelatedNodeTypeProvider _relatedNodeTypeProvider;
        [NotNull] private readonly IEnumerable<DiagramPluginId> _diagramPluginIds;

        [NotNull] private readonly Dictionary<DiagramId, IDiagramService> _diagramServices;
        [NotNull] private readonly Dictionary<DiagramId, IUiService> _diagramUis;

        // This dictionary is just to root the plugins.
        // ReSharper disable once CollectionNeverQueried.Local
        [NotNull]private readonly Dictionary<DiagramId, List<IDiagramPlugin>> _diagramPlugins;

        public VisualizationService(
            [NotNull] IModelService modelService,
            [NotNull] IDiagramServiceFactory diagramServiceFactory,
            [NotNull] IUiServiceFactory uiServiceFactory,
            [NotNull] IDiagramPluginFactory diagramPluginFactory,
            [NotNull] IRelatedNodeTypeProvider relatedNodeTypeProvider,
            [NotNull][ItemNotNull] IEnumerable<DiagramPluginId> diagramPluginIds)
        {
            _modelService = modelService;
            _diagramServiceFactory = diagramServiceFactory;
            _uiServiceFactory = uiServiceFactory;
            _diagramPluginFactory = diagramPluginFactory;
            _relatedNodeTypeProvider = relatedNodeTypeProvider;
            _diagramPluginIds = diagramPluginIds;

            _diagramServices = new Dictionary<DiagramId, IDiagramService>();
            _diagramUis = new Dictionary<DiagramId, IUiService>();
            _diagramPlugins = new Dictionary<DiagramId, List<IDiagramPlugin>>();
        }

        public DiagramId CreateDiagram(
            double minZoom = DefaultMinZoom,
            double maxZoom = DefaultMaxZoom,
            double initialZoom = DefaultInitialZoom)
        {
            var diagramId = DiagramId.Create();
            var diagramService = _diagramServiceFactory.Create(_modelService);
            _diagramServices.Add(diagramId, diagramService);

            var diagramUi = CreateDiagramUi(diagramId, minZoom, maxZoom, initialZoom);
            _diagramUis.Add(diagramId, diagramUi);

            // Warning: plugins must be created after the UI so its event callbacks don't precede UI updates.
            var diagramPlugins = CreateAndAttachDiagramPlugins(_diagramPluginIds, _modelService, diagramService);
            _diagramPlugins.Add(diagramId, diagramPlugins.ToList());

            return diagramId;
        }

        public IModelService GetModelService() => _modelService;
        public IDiagramService GetDiagramService(DiagramId diagramId) => _diagramServices[diagramId];
        public IUiService GetUiService(DiagramId diagramId) => _diagramUis[diagramId];

        [NotNull]
        private IUiService CreateDiagramUi(
            DiagramId diagramId,
            double minZoom = DefaultMinZoom,
            double maxZoom = DefaultMaxZoom,
            double initialZoom = DefaultInitialZoom)
        {
            var diagramService = GetDiagramService(diagramId);
            var diagramUi = _uiServiceFactory.Create(_modelService, diagramService, _relatedNodeTypeProvider, minZoom, maxZoom, initialZoom);

            diagramUi.DiagramNodePayloadAreaSizeChanged += (diagramNode, size) => OnDiagramNodePayloadAreaSizeChanged(diagramId, diagramNode, size);
            diagramUi.RemoveDiagramNodeRequested += diagramNode => OnRemoveDiagramNodeRequested(diagramId, diagramNode);

            return diagramUi;
        }

        [NotNull]
        [ItemNotNull]
        private IEnumerable<IDiagramPlugin> CreateAndAttachDiagramPlugins(
            IEnumerable<DiagramPluginId> diagramPluginIds,
            IModelService modelService,
            IDiagramService diagramService)
        {
            foreach (var diagramPluginId in diagramPluginIds)
            {
                var diagramPlugin = _diagramPluginFactory.Create(diagramPluginId);
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