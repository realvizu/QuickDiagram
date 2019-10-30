using System;
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
        [NotNull] private readonly IModelService _modelService;
        [NotNull] private readonly Func<IModel, IDiagramService> _diagramServiceFactory;
        [NotNull] private readonly Func<IDiagramService, IUiService> _uiServiceFactory;
        [NotNull] private readonly IEnumerable<Func<IDiagramService, IDiagramPlugin>> _diagramPluginFactories;

        [NotNull] private readonly Dictionary<DiagramId, IDiagramService> _diagramServices;
        [NotNull] private readonly Dictionary<DiagramId, IUiService> _diagramUis;
        [NotNull] private readonly Dictionary<DiagramId, List<IDiagramPlugin>> _diagramPlugins;

        public VisualizationService(
            [NotNull] IModelService modelService,
            [NotNull] Func<IModel, IDiagramService> diagramServiceFactory,
            [NotNull] Func<IDiagramService, IUiService> uiServiceFactory,
            [NotNull] IEnumerable<Func<IDiagramService, IDiagramPlugin>> diagramPluginFactories)
        {
            _modelService = modelService;
            _diagramServiceFactory = diagramServiceFactory;
            _uiServiceFactory = uiServiceFactory;
            _diagramPluginFactories = diagramPluginFactories;

            _diagramServices = new Dictionary<DiagramId, IDiagramService>();
            _diagramUis = new Dictionary<DiagramId, IUiService>();
            _diagramPlugins = new Dictionary<DiagramId, List<IDiagramPlugin>>();
        }

        public DiagramId CreateDiagram()
        {
            var diagramId = DiagramId.Create();

            var diagramService = _diagramServiceFactory(_modelService.LatestModel);
            _diagramServices.Add(diagramId, diagramService);

            var diagramUi = _uiServiceFactory(diagramService);
            diagramUi.DiagramNodePayloadAreaSizeChanged += PropagateDiagramNodePayloadAreaSizeChanged(diagramId);
            diagramUi.RemoveDiagramNodeRequested += PropagateRemoveDiagramNodeRequested(diagramId);
            _diagramUis.Add(diagramId, diagramUi);

            var plugins = _diagramPluginFactories.Select(i => i(diagramService)).ToList();
            _diagramPlugins.Add(diagramId, plugins);

            return diagramId;
        }

        public IModelService GetModelService() => _modelService;
        public IDiagramService GetDiagramService(DiagramId diagramId) => _diagramServices[diagramId];
        public IUiService GetUiService(DiagramId diagramId) => _diagramUis[diagramId];

        public void RemoveDiagram(DiagramId diagramId)
        {
            _diagramServices.Remove(diagramId);

            var diagramUi = _diagramUis[diagramId];
            diagramUi.DiagramNodePayloadAreaSizeChanged -= PropagateDiagramNodePayloadAreaSizeChanged(diagramId);
            diagramUi.RemoveDiagramNodeRequested -= PropagateRemoveDiagramNodeRequested(diagramId);
            _diagramUis.Remove(diagramId);

            _diagramPlugins[diagramId].ForEach(i => i.Dispose());
            _diagramPlugins.Remove(diagramId);
        }

        [NotNull]
        private Action<IDiagramNode> PropagateRemoveDiagramNodeRequested(DiagramId diagramId)
        {
            return diagramNode => OnRemoveDiagramNodeRequested(diagramId, diagramNode);
        }

        [NotNull]
        private Action<IDiagramNode, Size2D> PropagateDiagramNodePayloadAreaSizeChanged(DiagramId diagramId)
        {
            return (diagramNode, size) => OnDiagramNodePayloadAreaSizeChanged(diagramId, diagramNode, size);
        }

        private void OnDiagramNodePayloadAreaSizeChanged(DiagramId diagramId, [NotNull] IDiagramNode diagramNode, Size2D newSize)
        {
            GetDiagramService(diagramId).UpdateNodePayloadAreaSize(diagramNode.Id, newSize);
        }

        private void OnRemoveDiagramNodeRequested(DiagramId diagramId, [NotNull] IDiagramNode diagramNode)
        {
            GetDiagramService(diagramId).RemoveNode(diagramNode.Id);
        }
    }
}