using System;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Graph;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.UI.Extensibility;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// A diagram button to choose related entities.
    /// </summary>
    internal class ShowRelatedNodeButtonViewModel : DiagramButtonViewModelBase
    {
        private readonly RelatedEntityButtonDescriptor _descriptor;
        private List<IModelEntity> _relatedEntities;
        private List<IModelEntity> _displayedRelatedEntities;
        private List<IModelEntity> _undisplayedRelatedEntities;

        public ShowRelatedNodeButtonViewModel(IModel model, Diagram diagram,
            double buttonRadius, RelatedEntityButtonDescriptor descriptor)
            : base(model, diagram, buttonRadius, descriptor.ButtonLocation)
        {
            _descriptor = descriptor;
        }

        public ConnectorType ConnectorType => _descriptor.ConnectorType;

        private RelationshipSpecification RelationshipSpecification => _descriptor.RelationshipSpecification;
        private DiagramNodeViewModel2 AssociatedDiagramNodeViewModel => (DiagramNodeViewModel2)AssociatedDiagramShapeViewModel;
        private DiagramNode AssociatedDiagramNode => AssociatedDiagramNodeViewModel.DiagramNode;
        private IModelEntity AssociatedModelEntity => AssociatedDiagramNode.ModelEntity;

        public override void AssociateWith(DiagramShapeViewModelBase diagramShapeViewModel)
        {
            base.AssociateWith(diagramShapeViewModel);
            UpdateDisplayedEntityInfo();
        }

        protected override void OnClick()
        {
            if (_undisplayedRelatedEntities.Count == 1)
            {
                var modelEntity = _undisplayedRelatedEntities.First();
                Diagram.ShowItem(modelEntity);
                UpdateDisplayedEntityInfo();
            }
        }

        private void UpdateDisplayedEntityInfo()
        {
            _relatedEntities = Model.GetRelatedEntities(AssociatedModelEntity, RelationshipSpecification).ToList();
            _displayedRelatedEntities = _relatedEntities.Where(i => Diagram.Nodes.Any(j => j.ModelEntity == i)).ToList();
            _undisplayedRelatedEntities = _relatedEntities.Except(_displayedRelatedEntities).ToList();

            IsEnabled = _undisplayedRelatedEntities.Count > 0;
        }
    }
}
