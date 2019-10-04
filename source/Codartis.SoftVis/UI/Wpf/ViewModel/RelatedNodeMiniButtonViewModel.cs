using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Modeling.Definition;

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

        public RelatedNodeMiniButtonViewModel(IModelService modelService, IDiagramService diagramService,
            RelatedNodeType relatedNodeType)
            : base(modelService, diagramService, relatedNodeType.Name)
        {
            _directedModelRelationshipType = relatedNodeType.RelationshipType;
            ConnectorType = diagramService.GetConnectorType(relatedNodeType.RelationshipType.Stereotype);

            _lastModel = modelService.LatestModel;
            _lastDiagram = diagramService.LatestDiagram;

            ModelService.ModelChanged += OnModelChanged;
            DiagramService.DiagramChanged += OnDiagramChanged;
        }

        public override void Dispose()
        {
            base.Dispose();

            ModelService.ModelChanged -= OnModelChanged;
            DiagramService.DiagramChanged -= OnDiagramChanged;
        }

        public override object PlacementKey => _directedModelRelationshipType;

        private DiagramNodeViewModel HostDiagramNodeViewModel => HostViewModel as DiagramNodeViewModel;

        public override void AssociateWith(IDiagramShapeUi host)
        {
            base.AssociateWith(host);
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

        private void OnModelChanged(ModelEvent modelEvent)
        {
            _lastModel = modelEvent.NewModel;
            UpdateEnabledState();
        }

        private void OnDiagramChanged(DiagramEvent @event)
        {
            _lastDiagram = @event.NewDiagram;
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
            return _lastModel.GetRelatedNodes(modelNode.Id, _directedModelRelationshipType)
                .Except(_lastDiagram.Nodes.Select(j => j.ModelNode));
        }
    }
}
