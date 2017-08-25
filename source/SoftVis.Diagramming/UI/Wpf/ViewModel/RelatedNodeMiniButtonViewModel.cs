using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// A diagram button for choosing related model nodes.
    /// </summary>
    public class RelatedNodeMiniButtonViewModel : MiniButtonViewModelBase
    {
        private readonly DirectedModelRelationshipType _directedModelRelationshipType;
        public ConnectorType ConnectorType { get; }

        private IModel _lastModel;
        private IDiagram _lastDiagram;

        public RelatedNodeMiniButtonViewModel(IReadOnlyModelStore modelStore, IReadOnlyDiagramStore diagramStore,
            RelatedNodeType relatedNodeType)
            : base(modelStore, diagramStore, relatedNodeType.Name)
        {
            _directedModelRelationshipType = relatedNodeType.RelationshipType;
            ConnectorType = diagramStore.GetConnectorType(relatedNodeType.RelationshipType.Stereotype);

            _lastModel = modelStore.CurrentModel;
            _lastDiagram = diagramStore.CurrentDiagram;

            ModelStore.ModelChanged += OnModelChanged;
            DiagramStore.DiagramChanged += OnDiagramChanged;
        }

        public override void Dispose()
        {
            base.Dispose();

            ModelStore.ModelChanged -= OnModelChanged;
            DiagramStore.DiagramChanged -= OnDiagramChanged;
        }

        public override object PlacementKey => _directedModelRelationshipType;

        private DiagramNodeViewModelBase HostDiagramNodeViewModel => HostViewModel as DiagramNodeViewModelBase;

        public override void AssociateWith(DiagramShapeViewModelBase diagramNodeViewModel)
        {
            base.AssociateWith(diagramNodeViewModel);
            UpdateEnabledState();
        }

        protected override void OnClick()
        {
            // Copy to variable to avoid race condition of HostViewModel changing in-flight
            var hostDiagramNodeViewModel = HostDiagramNodeViewModel;
            if (hostDiagramNodeViewModel == null)
                return;

            var undisplayedRelatedModelNodes = GetUndisplayedRelatedModelNodes(hostDiagramNodeViewModel.ModelNode).ToList();

            if (undisplayedRelatedModelNodes.Count == 1)
            {
                hostDiagramNodeViewModel.ShowRelatedModelNodes(this, undisplayedRelatedModelNodes);
            }
            else if (undisplayedRelatedModelNodes.Count > 1)
            {
                hostDiagramNodeViewModel.ShowRelatedModelNodeSelector(this, undisplayedRelatedModelNodes);
            }
        }

        protected override void OnDoubleClick()
        {
            // Copy to variable to avoid race condition of HostViewModel changing in-flight
            var hostDiagramNodeViewModel = HostDiagramNodeViewModel;
            if (hostDiagramNodeViewModel == null)
                return;

            var undisplayedRelatedModelNodes = GetUndisplayedRelatedModelNodes(hostDiagramNodeViewModel.ModelNode);
            hostDiagramNodeViewModel.ShowRelatedModelNodes(this, undisplayedRelatedModelNodes.ToList());
        }

        private void OnModelChanged(ModelEventBase modelEvent)
        {
            _lastModel = modelEvent.NewModel;
            UpdateEnabledState();
        }

        private void OnDiagramChanged(DiagramEventBase diagramEvent)
        {
            _lastDiagram = diagramEvent.NewDiagram;
            UpdateEnabledState();
        }

        private void UpdateEnabledState()
        {
            // Copy to variable to avoid race condition of HostViewModel changing in-flight
            var hostDiagramNodeViewModel = HostDiagramNodeViewModel;
            if (hostDiagramNodeViewModel == null)
                return;

            IsEnabled = GetUndisplayedRelatedModelNodes(hostDiagramNodeViewModel.ModelNode).Any();
        }

        private IEnumerable<IModelNode> GetUndisplayedRelatedModelNodes(IModelNode modelNode)
        {
            return _lastModel.GetRelatedNodes(modelNode, _directedModelRelationshipType)
                .Except(_lastDiagram.Nodes.Select(j => j.ModelNode));
        }
    }
}
