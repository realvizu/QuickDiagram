using System.Linq;
using System.Windows;
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
        private const double VisualCueRadius = 2.5;

        private readonly RelatedEntitySpecification _relatedEntitySpecification;
        private readonly IDiagramNode _diagramNode;
        private readonly Size _parentSize;

        public RelatedEntityCueViewModel(IReadOnlyModel model, IDiagram diagram,
            RelatedEntityButtonDescriptor descriptor, IDiagramNode diagramNode, Size parentSize)
            : base(model, diagram, VisualCueRadius * 2, VisualCueRadius * 2, descriptor.ButtonLocation)
        {
            _relatedEntitySpecification = descriptor.RelatedEntitySpecification;
            _diagramNode = diagramNode;
            _parentSize = parentSize;
            // TODO !
            RelativeTopLeft = CalculateTopLeft();
            SubscribeToModelEvents();
            SubscribeToDiagramEvents();
            RecalculateVisibility();
        }

        protected override Size ParentSize => _parentSize;

        protected override Point CalculateTopLeft()
        {
            var buttonTopLeft = base.CalculateTopLeft();

            var originalVerticalTranslate = RectRelativeLocation.Translate.Y;
            var verticalTranslateAdjustment = originalVerticalTranslate < 0 
                ? originalVerticalTranslate + .5 
                : originalVerticalTranslate - .5;

            return buttonTopLeft - new Vector(0, verticalTranslateAdjustment);
        }

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
