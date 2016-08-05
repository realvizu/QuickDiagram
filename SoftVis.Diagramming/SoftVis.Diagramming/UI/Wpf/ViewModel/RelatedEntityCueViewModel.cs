using System.Linq;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.UI.Extensibility;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// View model for a visual cue that indicates the availability of related entities that are not on the diagram yet.
    /// </summary>
    public class RelatedEntityCueViewModel : DiagramShapeDecoratorViewModelBase
    {
        private readonly IDiagramNode _diagramNode;
        private readonly RelatedEntitySpecification _relatedEntitySpecification;

        public RelatedEntityCueViewModel(IReadOnlyModel model, IDiagram diagram,
            IDiagramNode diagramNode, RelatedEntityDescriptor descriptor)
            : base(model, diagram)
        {
            _diagramNode = diagramNode;
            _relatedEntitySpecification = descriptor.RelatedEntitySpecification;

            SubscribeToModelEvents();
            SubscribeToDiagramEvents();
            RecalculateVisibility();
        }

        /// <summary>
        /// The placement key is the RelatedEntitySpecification.
        /// </summary>
        public override object PlacementKey => _relatedEntitySpecification;

        private void SubscribeToModelEvents()
        {
            Model.RelationshipAdded += (sender, relationship) => RecalculateVisibility();
            Model.RelationshipRemoved += (sender, relationship) => RecalculateVisibility();
        }

        private void SubscribeToDiagramEvents()
        {
            Diagram.ShapeAdded += (sender, shape) => RecalculateVisibility();
            Diagram.ShapeRemoved += (sender, shape) => RecalculateVisibility();
        }

        private void RecalculateVisibility()
        {
            IsVisible = Diagram.GetUndisplayedRelatedEntities(_diagramNode, _relatedEntitySpecification).Any();
        }
    }
}
