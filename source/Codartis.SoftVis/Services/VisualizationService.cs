using System;
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
        [NotNull] private readonly IContainer _container;
        [NotNull] private readonly IModelService _modelService;

        [NotNull] private readonly Dictionary<DiagramId, ILifetimeScope> _diagramContainers;
        [NotNull] private readonly Dictionary<DiagramId, IDiagramService> _diagramServices;
        [NotNull] private readonly Dictionary<DiagramId, IUiService> _diagramUis;
        // ReSharper disable once CollectionNeverQueried.Local
        [NotNull] private readonly Dictionary<DiagramId, List<IDiagramPlugin>> _diagramPlugins;

        public VisualizationService(
            [NotNull] IContainer container,
            [NotNull] IModelService modelService)
        {
            _container = container;
            _modelService = modelService;

            _diagramContainers = new Dictionary<DiagramId, ILifetimeScope>();
            _diagramServices = new Dictionary<DiagramId, IDiagramService>();
            _diagramUis = new Dictionary<DiagramId, IUiService>();
            _diagramPlugins = new Dictionary<DiagramId, List<IDiagramPlugin>>();
        }

        public DiagramId CreateDiagram()
        {
            var diagramId = DiagramId.Create();

            var diagramContainer = _container.BeginLifetimeScope();
            _diagramContainers.Add(diagramId, diagramContainer);

            var diagramService = diagramContainer.Resolve<IDiagramService>(
                new TypedParameter(typeof(IModel), _modelService.LatestModel));
            _diagramServices.Add(diagramId, diagramService);

            var diagramUi = diagramContainer.Resolve<IUiService>(
                new TypedParameter(typeof(IDiagramService), diagramService));
            diagramUi.DiagramNodePayloadAreaSizeChanged += PropagateDiagramNodePayloadAreaSizeChanged(diagramId);
            diagramUi.RemoveDiagramNodeRequested += PropagateRemoveDiagramNodeRequested(diagramId);
            _diagramUis.Add(diagramId, diagramUi);

            var plugins = diagramContainer.Resolve<IEnumerable<IDiagramPlugin>>(
                    new TypedParameter(typeof(IDiagramService), diagramService))
                .ToList();
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

            _diagramPlugins.Remove(diagramId);

            _diagramContainers[diagramId].Dispose();
            _diagramContainers.Remove(diagramId);
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