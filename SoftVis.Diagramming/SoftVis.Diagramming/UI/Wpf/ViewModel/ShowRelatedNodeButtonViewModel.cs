using System.Collections.Generic;
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

        public event ShowRelatedNodeButtonEventHandler EntitySelectorRequested;
        public event ShowRelatedNodeButtonEventHandler ShowRelatedEntitiesRequested;

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
        private IDiagramNode HostDiagramNode => HostViewModel?.DiagramNode;

        /// <summary>
        /// For related entity buttons the placement key is the RelatedEntitySpecification.
        /// </summary>
        public override object PlacementKey => EntityRelationType;

        public override void AssociateWith(DiagramNodeViewModel diagramNodeViewModel)
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
            if (HostDiagramNode == null)
                return;

            IsEnabled = GetUndisplayedRelatedModelEntities().Any();
        }

        private IReadOnlyList<IModelEntity> GetUndisplayedRelatedModelEntities()
            => Diagram.GetUndisplayedRelatedModelEntities(HostDiagramNode, EntityRelationType);
    }
}
