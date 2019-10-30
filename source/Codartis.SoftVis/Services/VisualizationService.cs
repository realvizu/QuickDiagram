using System.Collections.Generic;
using System.Linq;
using Autofac;
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

        [NotNull] private readonly IContainer _container;
        [NotNull] private readonly IModelService _modelService;

        // This dictionary is just to root the per-diagram containers.
        // ReSharper disable once CollectionNeverQueried.Local
        [NotNull] private readonly Dictionary<DiagramId, ILifetimeScope> _diagramContainers;

        // TODO: remove?
        [NotNull] private readonly Dictionary<DiagramId, IDiagramService> _diagramServices;
        [NotNull] private readonly Dictionary<DiagramId, IUiService> _diagramUis;

        // This dictionary is just to root the plugins.
        // ReSharper disable once CollectionNeverQueried.Local
        [NotNull] private readonly Dictionary<DiagramId, List<IDiagramPlugin>> _diagramPlugins;

        public VisualizationService(
            [NotNull] IContainer container,
            [NotNull] IModelService modelService)
        {
            _container = container;
            _modelService = modelService;

            _diagramContainers = new Dictionary<DiagramId, ILifetimeScope>();

            // TODO: remove?
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

            var diagramContainer = _container.BeginLifetimeScope();
            _diagramContainers.Add(diagramId, diagramContainer);

            var diagramService = diagramContainer.Resolve<IDiagramService>(
                new TypedParameter(typeof(IModel), _modelService.LatestModel)
            );
            _diagramServices.Add(diagramId, diagramService);

            var diagramUi = diagramContainer.Resolve<IUiService>(
                new TypedParameter(typeof(IDiagramService), diagramService)
            );
            diagramUi.DiagramNodePayloadAreaSizeChanged += (diagramNode, size) => OnDiagramNodePayloadAreaSizeChanged(diagramId, diagramNode, size);
            diagramUi.RemoveDiagramNodeRequested += diagramNode => OnRemoveDiagramNodeRequested(diagramId, diagramNode);
            _diagramUis.Add(diagramId, diagramUi);

            var plugins = diagramContainer.Resolve<IEnumerable<IDiagramPlugin>>().ToList();
            _diagramPlugins.Add(diagramId, plugins);

            foreach (var plugin in plugins)
                plugin.Initialize(_modelService, diagramService);

            return diagramId;
        }

        public IModelService GetModelService() => _modelService;
        public IDiagramService GetDiagramService(DiagramId diagramId) => _diagramServices[diagramId];
        public IUiService GetUiService(DiagramId diagramId) => _diagramUis[diagramId];

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