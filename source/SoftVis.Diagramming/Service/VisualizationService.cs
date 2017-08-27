using System;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.UI;

namespace Codartis.SoftVis.Service
{
    /// <summary>
    /// Implements visualization operations.
    /// </summary>
    public class VisualizationService : IVisualizationService
    {
        private const double DefaultMinZoom = .1;
        private const double DefaultMaxZoom = 10;
        private const double DefaultInitialZoom = 1;

        protected readonly IModelStoreFactory ModelStoreFactory;
        protected readonly IDiagramStoreFactory DiagramStoreFactory;
        protected readonly IDiagramShapeFactory DiagramShapeFactory;
        protected readonly IDiagramUiFactory DiagramUiFactory;
        protected readonly IDiagramShapeUiFactory DiagramShapeUiFactory;
        protected readonly IDiagramPluginFactory DiagramPluginFactory;
        protected readonly IEnumerable<DiagramPluginId> DiagramPluginIds;

        protected readonly IModelStore ModelStore;
        private readonly Dictionary<DiagramId, IDiagramStore> _diagramStores;
        private readonly Dictionary<DiagramId, IDiagramUi> _diagramUis;
        private readonly Dictionary<DiagramId, List<IDiagramPlugin>> _diagramPlugins;

        public event Action<IModelNode> ModelNodeInvoked;

        public VisualizationService(
            IModelStoreFactory modelStoreFactory,
            IDiagramStoreFactory diagramStoreFactory,
            IDiagramShapeFactory diagramShapeFactory,
            IDiagramUiFactory diagramUiFactory,
            IDiagramShapeUiFactory diagramShapeUiFactory,
            IDiagramPluginFactory diagramPluginFactory,
            IEnumerable<DiagramPluginId> diagramPluginIds)
        {
            ModelStoreFactory = modelStoreFactory;
            DiagramStoreFactory = diagramStoreFactory;
            DiagramPluginFactory = diagramPluginFactory;
            DiagramShapeFactory = diagramShapeFactory;
            DiagramUiFactory = diagramUiFactory;
            DiagramShapeUiFactory = diagramShapeUiFactory;
            DiagramPluginIds = diagramPluginIds;

            ModelStore = ModelStoreFactory.Create();
            _diagramStores = new Dictionary<DiagramId, IDiagramStore>();
            _diagramUis = new Dictionary<DiagramId, IDiagramUi>();
            _diagramPlugins = new Dictionary<DiagramId, List<IDiagramPlugin>>();
        }


        public IModelStore GetModelStore() => ModelStore;

        public DiagramId CreateDiagram()
        {
            var newDiagramId = DiagramId.Create();
            var newDiagramStore = DiagramStoreFactory.Create();
            _diagramStores.Add(newDiagramId, newDiagramStore);

            var diagramPlugins = CreateAndAttachDiagramPlugins(DiagramPluginIds, ModelStore, newDiagramStore);
            _diagramPlugins.Add(newDiagramId, diagramPlugins.ToList());

            return newDiagramId;
        }

        public IDiagramUi CreateDiagramUi(DiagramId diagramId,
            double minZoom = DefaultMinZoom,
            double maxZoom = DefaultMaxZoom,
            double initialZoom = DefaultInitialZoom)
        {
            var diagramStore = GetDiagramStore(diagramId);
            var diagramUi = DiagramUiFactory.Create(ModelStore, diagramStore, DiagramShapeUiFactory,
                minZoom, maxZoom, initialZoom);

            diagramUi.DiagramNodeSizeChanged += (node, size) => OnDiagramNodeSizeChanged(diagramId, node, size);
            diagramUi.DiagramNodeInvoked += node => OnDiagramNodeInvoked(diagramId, node);

            _diagramUis.Add(diagramId, diagramUi);
            return diagramUi;
        }

        public void ShowModelNode(DiagramId diagramId, IModelNode modelNode)
        {
            var diagramStore = GetDiagramStore(diagramId);

            var diagram = diagramStore.CurrentDiagram;
            if (diagram.Nodes.Any(i => i.ModelNode == modelNode))
                return;

            var diagramNode = DiagramShapeFactory.CreateDiagramNode(diagramStore, modelNode);
            diagramStore.AddNode(diagramNode);
        }

        public void ShowModelRelationship(DiagramId diagramId, IModelRelationship modelRelationship)
        {
            var diagramStore = GetDiagramStore(diagramId);

            var diagram = diagramStore.CurrentDiagram;
            if (diagram.Connectors.Any(i => i.ModelRelationship == modelRelationship))
                return;

            var diagramConnector = DiagramShapeFactory.CreateDiagramConnector(diagramStore, modelRelationship);
            diagramStore.AddConnector(diagramConnector);
        }

        public void HideModelNode(DiagramId diagramId, IModelNode modelNode)
        {
            throw new NotImplementedException();
        }

        public void HideModelRelationship(DiagramId diagramId, IModelRelationship modelRelationship)
        {
            throw new NotImplementedException();
        }

        public void ClearDiagram(DiagramId diagramId)
        {
            throw new NotImplementedException();
        }

        public void ClearModel()
        {
            throw new NotImplementedException();
        }

        public void UpdateModelFromSource()
        {
            throw new NotImplementedException();
        }

        protected IDiagramStore GetDiagramStore(DiagramId diagramId) => _diagramStores[diagramId];

        private IEnumerable<IDiagramPlugin> CreateAndAttachDiagramPlugins(IEnumerable<DiagramPluginId> diagramPluginIds,
            IModelStore modelStore, IDiagramStore diagramStore)
        {
            foreach (var diagramPluginId in diagramPluginIds)
            {
                var diagramPlugin = DiagramPluginFactory.Create(diagramPluginId);
                diagramPlugin.Initialize(modelStore, diagramStore);
                yield return diagramPlugin;
            }
        }

        private void OnDiagramNodeSizeChanged(DiagramId diagramId, IDiagramNode diagramNode, Size2D newSize)
        {
            GetDiagramStore(diagramId).UpdateDiagramNodeSize(diagramNode, newSize);
        }

        private void OnDiagramNodeInvoked(DiagramId diagramId, IDiagramNode diagramNode)
        {
            ModelNodeInvoked?.Invoke(diagramNode.ModelNode);
        }
    }
}
