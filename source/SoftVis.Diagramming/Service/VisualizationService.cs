using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.UI;
using Codartis.SoftVis.UI.Wpf.View;
using Codartis.SoftVis.UI.Wpf.ViewModel;
using Codartis.SoftVis.Util;

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
        private IDiagramStlyeProvider _diagramStlyeProvider;
        private ResourceDictionary _resourceDictionary;

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

        public void InitializeUi(IDiagramStlyeProvider diagramStlyeProvider, ResourceDictionary resourceDictionary)
        {
            _diagramStlyeProvider = diagramStlyeProvider;
            _resourceDictionary = resourceDictionary;
        }

        public IModelStore GetModelStore() => ModelStore;

        public void ClearModel() => ModelStore.ClearModel();

        public void UpdateModelFromSource() => throw new NotImplementedException();

        public DiagramId CreateDiagram(
            double minZoom = DefaultMinZoom,
            double maxZoom = DefaultMaxZoom,
            double initialZoom = DefaultInitialZoom)
        {
            var newDiagramId = DiagramId.Create();
            var newDiagramStore = DiagramStoreFactory.Create();
            _diagramStores.Add(newDiagramId, newDiagramStore);

            var diagramUi = CreateDiagramUi(newDiagramId, minZoom, maxZoom, initialZoom);
            _diagramUis.Add(newDiagramId, diagramUi);

            // Warning: plugins must be created after the UI so its event callbacks don't precede UI updates.
            var diagramPlugins = CreateAndAttachDiagramPlugins(DiagramPluginIds, ModelStore, newDiagramStore);
            _diagramPlugins.Add(newDiagramId, diagramPlugins.ToList());

            return newDiagramId;
        }

        public IDiagramUi GetDiagramUi(DiagramId diagramId) => _diagramUis[diagramId];

        public void ShowModelNode(DiagramId diagramId, IModelNode modelNode)
        {
            var diagramStore = GetDiagramStore(diagramId);

            var diagram = diagramStore.CurrentDiagram;
            if (diagram.NodeExistsById(modelNode.Id))
                return;

            var diagramNode = DiagramShapeFactory.CreateDiagramNode(diagramStore, modelNode);
            diagramStore.AddNode(diagramNode);
        }

        public void ShowModelRelationship(DiagramId diagramId, IModelRelationship modelRelationship)
        {
            var diagramStore = GetDiagramStore(diagramId);

            var diagram = diagramStore.CurrentDiagram;
            if (diagram.ConnectorExistsById(modelRelationship.Id))
                return;

            var diagramConnector = DiagramShapeFactory.CreateDiagramConnector(diagramStore, modelRelationship);
            diagramStore.AddConnector(diagramConnector);
        }

        public void HideModelNode(DiagramId diagramId, IModelNode modelNode)
        {
            var diagramStore = GetDiagramStore(diagramId);

            var diagram = diagramStore.CurrentDiagram;
            if (diagram.TryGetNodeById(modelNode.Id, out IDiagramNode diagramNode))
            {
                var diagramConnectors = diagram.GetConnectorsByNodeId(modelNode.Id);
                foreach (var diagramConnector in diagramConnectors)
                    diagramStore.RemoveConnector(diagramConnector);

                diagramStore.RemoveNode(diagramNode);
            }
        }

        public void HideModelRelationship(DiagramId diagramId, IModelRelationship modelRelationship)
        {
            var diagramStore = GetDiagramStore(diagramId);

            var diagram = diagramStore.CurrentDiagram;
            if (diagram.TryGetConnectorById(modelRelationship.Id, out IDiagramConnector diagramConnector))
                diagramStore.RemoveConnector(diagramConnector);
        }

        public void ClearDiagram(DiagramId diagramId)
        {
            var diagramStore = GetDiagramStore(diagramId);
            diagramStore.ClearDiagram();
        }

        public void ZoomToContent(DiagramId diagramId) => GetDiagramUi(diagramId).ZoomToContent();

        public async Task<BitmapSource> CreateDiagramImageAsync(DiagramId diagramId, double dpi, double margin,
            CancellationToken cancellationToken = default(CancellationToken),
            IIncrementalProgress progress = null, IProgress<int> maxProgress = null)
        {
            try
            {
                var diagramUi = GetDiagramUi(diagramId);
                var diagramImageCreator = new DataCloningDiagramImageCreator((DiagramViewModel)diagramUi,
                    _diagramStlyeProvider, _resourceDictionary);

                return await Task.Factory.StartSTA(() =>
                    diagramImageCreator.CreateImage(dpi, margin, cancellationToken, progress, maxProgress), cancellationToken);
            }
            catch (OperationCanceledException)
            {
                return null;
            }
        }

        private IDiagramStore GetDiagramStore(DiagramId diagramId) => _diagramStores[diagramId];

        private IDiagramUi CreateDiagramUi(DiagramId diagramId,
            double minZoom = DefaultMinZoom,
            double maxZoom = DefaultMaxZoom,
            double initialZoom = DefaultInitialZoom)
        {
            var diagramStore = GetDiagramStore(diagramId);
            var diagramUi = DiagramUiFactory.Create(ModelStore, diagramStore, DiagramShapeUiFactory,
                minZoom, maxZoom, initialZoom);

            diagramUi.ShowModelItemsRequested += (modelNodes, followNewDiagramNodes) => OnShowModelItemsRequested(diagramId, modelNodes, followNewDiagramNodes);
            diagramUi.DiagramNodeSizeChanged += (diagramNode, size) => OnDiagramNodeSizeChanged(diagramId, diagramNode, size);
            diagramUi.DiagramNodeInvoked += node => OnDiagramNodeInvoked(diagramId, node);
            diagramUi.RemoveDiagramNodeRequested += diagramNode => OnRemoveDiagramNodeRequested(diagramId, diagramNode);

            return diagramUi;
        }

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

        private void OnShowModelItemsRequested(DiagramId diagramId, IReadOnlyList<IModelNode> modelNodes, bool followNewDiagramNodes)
        {
            foreach (var modelNode in modelNodes)
                ShowModelNode(diagramId, modelNode);

            if (followNewDiagramNodes)
            {
                var diagramStore = GetDiagramStore(diagramId);
                var diagramNodes = modelNodes.Select(i => diagramStore.GetDiagramNodeById(i.Id)).ToArray();
                GetDiagramUi(diagramId).FollowDiagramNodes(diagramNodes);
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

        private void OnRemoveDiagramNodeRequested(DiagramId diagramId, IDiagramNode diagramNode)
        {
            HideModelNode(diagramId, diagramNode.ModelNode);
        }
    }
}
