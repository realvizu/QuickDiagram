using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling2;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// A diagram button to choose related entities.
    /// </summary>
    public class ShowRelatedNodeButtonViewModel : DiagramShapeButtonViewModelBase
    {
        private readonly DirectedModelRelationshipType _relationshipType;

        public event ShowRelatedNodeButtonEventHandler EntitySelectorRequested;
        public event ShowRelatedNodeButtonEventHandler ShowRelatedEntitiesRequested;

        public ShowRelatedNodeButtonViewModel(IArrangedDiagram diagram, RelatedNodeType relatedNodeType)
            : base(diagram, relatedNodeType.Name)
        {
            _relationshipType = relatedNodeType.RelationshipType;
            SubscribeToModelEvents();
        }

        public override void Dispose()
        {
            UnsubscribeFromModelEvents();
        }

        public ConnectorType ConnectorType => Diagram.GetConnectorType(_relationshipType.Type);

        private IDiagramNode HostDiagramNode => HostViewModel?.DiagramNode;

        /// <summary>
        /// For related entity buttons the placement key is a DirectedModelRelationshipType.
        /// </summary>
        public override object PlacementKey => _relationshipType;

        public override void AssociateWith(DiagramNodeViewModelBase diagramNodeViewModel)
        {
            base.AssociateWith(diagramNodeViewModel);
            UpdateEnabledState();
        }

        protected override void OnClick()
        {
            if (HostDiagramNode == null)
            {
#if DEBUG
                Debugger.Break();
#else
                return;
#endif
            }

            var undisplayedRelatedEntities = GetUndisplayedRelatedModelEntities();

            if (undisplayedRelatedEntities.Count == 1)
            {
                ShowRelatedEntitiesRequested?.Invoke(this, undisplayedRelatedEntities);
            }
            else if (undisplayedRelatedEntities.Count > 1)
            {
                EntitySelectorRequested?.Invoke(this, undisplayedRelatedEntities);
            }
        }

        protected override void OnDoubleClick()
        {
            var undisplayedRelatedEntities = GetUndisplayedRelatedModelEntities();
            ShowRelatedEntitiesRequested?.Invoke(this, undisplayedRelatedEntities);
        }

        private void OnModelRelationshipAdded(IModelRelationship relationship) => UpdateEnabledState();
        private void OnModelRelationshipRemoved(IModelRelationship relationship) => UpdateEnabledState();

        private void SubscribeToModelEvents()
        {
            //Model.RelationshipAdded +=  OnModelRelationshipAdded;
            //Model.RelationshipRemoved += OnModelRelationshipRemoved;
        }

        private void UnsubscribeFromModelEvents()
        {
            //Model.RelationshipAdded -= OnModelRelationshipAdded;
            //Model.RelationshipRemoved -= OnModelRelationshipRemoved;
        }

        private void UpdateEnabledState()
        {
            if (HostDiagramNode == null)
                return;

            IsEnabled = GetUndisplayedRelatedModelEntities().Any();
        }

        private IReadOnlyList<IModelNode> GetUndisplayedRelatedModelEntities() => 
            Diagram.GetUndisplayedRelatedModelNodes(HostDiagramNode, _relationshipType);
    }
}
