using System.Linq;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// View model for a visual cue that indicates the availability of related entities that are not on the diagram yet.
    /// </summary>
    public class RelatedEntityCueViewModel : DiagramShapeDecoratorViewModelBase
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

        /// <summary>
        /// The placement key is the RelatedEntitySpecification.
        /// </summary>
        public override object PlacementKey => _entityRelationType;

        private void SubscribeToModelEvents()
        {
            Model.RelationshipAdded += (sender, relationship) => RecalculateVisibility();
            Model.RelationshipRemoved += (sender, relationship) => RecalculateVisibility();
        }

        private void SubscribeToDiagramEvents()
        {
            Diagram.ShapeAdded += shape => RecalculateVisibility();
            Diagram.ShapeRemoved += shape => RecalculateVisibility();
        }

        private void RecalculateVisibility()
        {
            IsVisible = Diagram.GetUndisplayedRelatedEntities(_diagramNode, _entityRelationType).Any();
        }
    }
}
