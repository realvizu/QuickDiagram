using System;
using System.Linq;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// View model for a visual cue that indicates the availability of related entities that are not on the diagram yet.
    /// </summary>
    public class RelatedEntityCueViewModel : DiagramShapeDecoratorViewModelBase, IDisposable
    {
        private readonly IDiagramNode _diagramNode;
        private readonly EntityRelationType _entityRelationType;

        public RelatedEntityCueViewModel(IArrangedDiagram diagram, IDiagramNode diagramNode, EntityRelationType descriptor)
            : base(diagram)
        {
            _diagramNode = diagramNode;
            _entityRelationType = descriptor;

            SubscribeToModelEvents();
            SubscribeToDiagramEvents();
            RecalculateVisibility();
        }

        public void Dispose()
        {
            UnsubscribeFromModelEvents();
            UnsubscribeFromDiagramEvents();
        }

        /// <summary>
        /// The placement key is the RelatedEntitySpecification.
        /// </summary>
        public override object PlacementKey => _entityRelationType;

        private void RecalculateVisibility()
        {
            IsVisible = Diagram.GetUndisplayedRelatedEntities(_diagramNode, _entityRelationType).Any();
        }

        private void OnModelRelationshipRemoved(object sender, IModelRelationship relationship) => RecalculateVisibility();
        private void OnModelRelationshipAdded(object sender, IModelRelationship relationship) => RecalculateVisibility();

        private void OnDiagramShapeRemoved(IDiagramShape shape) => RecalculateVisibility();
        private void OnDiagramShapeAdded(IDiagramShape shape) => RecalculateVisibility();

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

        private void SubscribeToDiagramEvents()
        {
            Diagram.ShapeAdded += OnDiagramShapeAdded;
            Diagram.ShapeRemoved += OnDiagramShapeRemoved;
        }

        private void UnsubscribeFromDiagramEvents()
        {
            Diagram.ShapeAdded -= OnDiagramShapeAdded;
            Diagram.ShapeRemoved -= OnDiagramShapeRemoved;
        }
    }
}
