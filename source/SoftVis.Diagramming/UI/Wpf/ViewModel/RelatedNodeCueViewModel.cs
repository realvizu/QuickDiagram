using System;
using System.Linq;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// View model for a visual cue that indicates the availability of related nodes that are not on the diagram yet.
    /// </summary>
    public class RelatedNodeCueViewModel : DiagramShapeDecoratorViewModelBase, IDisposable
    {
        private readonly IDiagramNode _diagramNode;
        private readonly DirectedModelRelationshipType _modelRelationshipType;

        public RelatedNodeCueViewModel(IArrangedDiagram diagram, IDiagramNode diagramNode, RelatedNodeType relatedNodeType)
            : base(diagram)
        {
            _diagramNode = diagramNode;
            _modelRelationshipType = relatedNodeType.RelationshipType;

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
        /// The placement key is the directed relationship type.
        /// </summary>
        public override object PlacementKey => _modelRelationshipType;

        private void RecalculateVisibility()
        {
            IsVisible = Diagram.GetUndisplayedRelatedModelNodes(_diagramNode, _modelRelationshipType).Any();
        }

        private void OnModelRelationshipRemoved(IModelRelationship relationship) => RecalculateVisibility();
        private void OnModelRelationshipAdded(IModelRelationship relationship) => RecalculateVisibility();

        private void OnDiagramShapeRemoved(IDiagramShape shape) => RecalculateVisibility();
        private void OnDiagramShapeAdded(IDiagramShape shape) => RecalculateVisibility();

        private void SubscribeToModelEvents()
        {
            //Model.RelationshipAdded += OnModelRelationshipAdded;
            //Model.RelationshipRemoved += OnModelRelationshipRemoved;
        }

        private void UnsubscribeFromModelEvents()
        {
            //Model.RelationshipAdded -= OnModelRelationshipAdded;
            //Model.RelationshipRemoved -= OnModelRelationshipRemoved;
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
