using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling2;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// A diagram button for choosing related model nodes.
    /// </summary>
    public class RelatedNodeMiniButtonViewModel : MiniButtonViewModelBase
    {
        private readonly DirectedModelRelationshipType _relationshipType;

        public RelatedNodeMiniButtonViewModel(IArrangedDiagram diagram, RelatedNodeType relatedNodeType)
            : base(diagram, relatedNodeType.Name)
        {
            _relationshipType = relatedNodeType.RelationshipType;
            SubscribeToModelEvents();
        }

        public override void Dispose()
        {
            UnsubscribeFromModelEvents();
        }

        public ConnectorType ConnectorType => Diagram.GetConnectorType(_relationshipType.Stereotype);

        private DiagramNodeViewModelBase HostDiagramNodeViewModel => HostViewModel as DiagramNodeViewModelBase;
        private IDiagramNode HostDiagramNode => HostDiagramNodeViewModel?.DiagramNode;

        /// <summary>
        /// For related entity buttons the placement key is the directed relationship type.
        /// </summary>
        public override object PlacementKey => _relationshipType;

        public override void AssociateWith(DiagramShapeViewModelBase diagramNodeViewModel)
        {
            base.AssociateWith(diagramNodeViewModel);
            UpdateEnabledState();
        }

        protected override void OnClick()
        {
            if (HostDiagramNodeViewModel == null)
            {
#if DEBUG
                Debugger.Break();
#else
                return;
#endif
            }

            var undisplayedRelatedEntities = GetUndisplayedRelatedModelNodes();

            if (undisplayedRelatedEntities.Count == 1)
            {
                HostDiagramNodeViewModel.ShowRelatedNodes(this, undisplayedRelatedEntities);
            }
            else if (undisplayedRelatedEntities.Count > 1)
            {
                HostDiagramNodeViewModel.ShowRelatedNodeSelector(this, undisplayedRelatedEntities);
            }
        }

        protected override void OnDoubleClick()
        {
            var undisplayedRelatedEntities = GetUndisplayedRelatedModelNodes();
            HostDiagramNodeViewModel.ShowRelatedNodes(this, undisplayedRelatedEntities);
        }

        private void OnModelRelationshipAdded(IModelRelationship relationship, IModel model) => UpdateEnabledState();
        private void OnModelRelationshipRemoved(IModelRelationship relationship, IModel model) => UpdateEnabledState();

        private void SubscribeToModelEvents()
        {
            Model.RelationshipAdded += OnModelRelationshipAdded;
            Model.RelationshipRemoved += OnModelRelationshipRemoved;
        }

        private void UnsubscribeFromModelEvents()
        {
            Model.RelationshipAdded -= OnModelRelationshipAdded;
            Model.RelationshipRemoved -= OnModelRelationshipRemoved;
        }

        private void UpdateEnabledState()
        {
            if (HostDiagramNode == null)
                return;

            IsEnabled = GetUndisplayedRelatedModelNodes().Any();
        }

        private IReadOnlyList<IModelNode> GetUndisplayedRelatedModelNodes() => 
            Diagram.GetUndisplayedRelatedModelNodes(HostDiagramNode, _relationshipType);
    }
}
