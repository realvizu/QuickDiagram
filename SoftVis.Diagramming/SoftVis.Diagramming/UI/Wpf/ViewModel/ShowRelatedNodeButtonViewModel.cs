using System.Diagnostics;
using System.Linq;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// A diagram button to choose related entities.
    /// </summary>
    public class ShowRelatedNodeButtonViewModel : DiagramShapeButtonViewModelBase
    {
        private readonly EntityRelationType _relationType;

        public event EntitySelectorRequestedEventHandler EntitySelectorRequested;

        public ShowRelatedNodeButtonViewModel(IArrangedDiagram diagram, EntityRelationType relationType)
            : base(diagram)
        {
            _relationType = relationType;
            SubscribeToModelEvents();
        }

        public override void Dispose()
        {
            UnsubscribeFromModelEvents();
        }

        public ConnectorType ConnectorType => Diagram.GetConnectorType(_relationType.Type);

        private EntityRelationType EntityRelationType => _relationType;
        private DiagramNodeViewModel AssociatedDiagramNodeViewModel => (DiagramNodeViewModel)AssociatedDiagramShapeViewModel;
        private IDiagramNode AssociatedDiagramNode => AssociatedDiagramNodeViewModel?.DiagramNode;

        /// <summary>
        /// For related entity buttons the placement key is the RelatedEntitySpecification.
        /// </summary>
        public override object PlacementKey => EntityRelationType;

        public override void AssociateWith(DiagramShapeViewModelBase diagramShapeViewModel)
        {
            base.AssociateWith(diagramShapeViewModel);
            UpdateEnabledState();
        }

        protected override void OnClick()
        {
            if (AssociatedDiagramNode == null)
            {
                // TODO: find out how this happens
                //Debugger.Break();
                return;
            }

            var undisplayedRelatedEntities = Diagram.GetUndisplayedRelatedEntities(
                AssociatedDiagramNode, EntityRelationType).ToList();

            if (undisplayedRelatedEntities.Count == 1)
            {
                Diagram.ShowItem(undisplayedRelatedEntities.First());
            }
            else if (undisplayedRelatedEntities.Count > 1)
            {
                EntitySelectorRequested?.Invoke(this, undisplayedRelatedEntities);
            }
        }

        protected override void OnDoubleClick()
        {
            var undisplayedRelatedEntities = Diagram.GetUndisplayedRelatedEntities(AssociatedDiagramNode, EntityRelationType);
            Diagram.ShowItems(undisplayedRelatedEntities);
        }

        private void OnModelRelationshipAdded(object sender, IModelRelationship relationship) => UpdateEnabledState();
        private void OnModelRelationshipRemoved(object sender, IModelRelationship relationship) => UpdateEnabledState();

        private void SubscribeToModelEvents()
        {
            Model.RelationshipAdded +=  OnModelRelationshipAdded;
            Model.RelationshipRemoved += OnModelRelationshipRemoved;
        }

        private void UnsubscribeFromModelEvents()
        {
            Model.RelationshipAdded -= OnModelRelationshipAdded;
            Model.RelationshipRemoved -= OnModelRelationshipRemoved;
        }

        private void UpdateEnabledState()
        {
            if (AssociatedDiagramNode == null)
                return;

            IsEnabled = Diagram.GetUndisplayedRelatedEntities(AssociatedDiagramNode, EntityRelationType).Any();
        }
    }
}
