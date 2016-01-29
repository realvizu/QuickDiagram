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
        private List<DiagramNode> _relatedNodes;

        public ShowRelatedNodeButtonViewModel(IModel model, Diagram diagram,
            double buttonRadius, RelatedEntityButtonDescriptor descriptor)
            : base(model, diagram, buttonRadius, descriptor.ButtonLocation, i => i.Remove())
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
            var diagramNodeViewModel = diagramShapeViewModel as DiagramNodeViewModel2;
            if (diagramNodeViewModel == null)
                throw new ArgumentException($"Must be associated with diagram node.");

            base.AssociateWith(diagramShapeViewModel);

            _relatedEntities = Model.GetRelatedEntities(AssociatedModelEntity, RelationshipSpecification).ToList();
            _relatedNodes = Diagram.GetRelatedNodes(AssociatedDiagramNode, RelationshipSpecification).ToList();

            IsEnabled = HasUndisplayedRelatedEntity();
        }

        private bool HasUndisplayedRelatedEntity()
        {
            return _relatedEntities.Count > _relatedNodes.Count;
        }
    }
}
