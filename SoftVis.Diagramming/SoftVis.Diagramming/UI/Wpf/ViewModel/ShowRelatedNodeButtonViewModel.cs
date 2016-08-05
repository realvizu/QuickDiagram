using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.UI.Extensibility;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// A diagram button to choose related entities.
    /// </summary>
    internal class ShowRelatedNodeButtonViewModel : DiagramShapeButtonViewModelBase
    {
        private readonly RelatedEntityDescriptor _descriptor;

        public event EntitySelectorRequestedEventHandler EntitySelectorRequested;

        public ShowRelatedNodeButtonViewModel(IReadOnlyModel model, IDiagram diagram,
            RelatedEntityDescriptor descriptor)
            : base(model, diagram)
        {
            _descriptor = descriptor;
            SubscribeToModelEvents();
        }

        public ConnectorType ConnectorType => _descriptor.ConnectorType;

        private RelatedEntitySpecification RelatedEntitySpecification => _descriptor.RelatedEntitySpecification;
        private DiagramNodeViewModel AssociatedDiagramNodeViewModel => (DiagramNodeViewModel)AssociatedDiagramShapeViewModel;
        private IDiagramNode AssociatedDiagramNode => AssociatedDiagramNodeViewModel?.DiagramNode;

        /// <summary>
        /// For related entity buttons the placement key is the RelatedEntitySpecification.
        /// </summary>
        public override object PlacementKey => RelatedEntitySpecification;

        public override void AssociateWith(DiagramShapeViewModelBase diagramShapeViewModel)
        {
            base.AssociateWith(diagramShapeViewModel);
            UpdateEnabledState();
        }

        protected override void OnClick()
        {
            var undisplayedRelatedEntities = Diagram.GetUndisplayedRelatedEntities(
                AssociatedDiagramNode, RelatedEntitySpecification).ToList();

            if (undisplayedRelatedEntities.Count == 1)
            {
                Diagram.ShowItem(undisplayedRelatedEntities.First());
            }
            else if (undisplayedRelatedEntities.Count > 1)
            {
                RaiseEntitySelectorRequest(undisplayedRelatedEntities);
            }
        }

        private void RaiseEntitySelectorRequest(IEnumerable<IModelEntity> undisplayedRelatedEntities)
        {
            var handleOrientation = HandleOrientation.Bottom;// CalculateHandleOrientation(ButtonLocation);
            var parentNodePositionVector = (Vector)AssociatedDiagramNodeViewModel.Position;
           // var rectInDiagramSpace = RelativeRect.Add(parentNodePositionVector);
           // var attachPointInDiagramSpace = CalculateAttachPoint(rectInDiagramSpace, handleOrientation);

            // TODO: leave position calculation to view?
            EntitySelectorRequested?.Invoke((Point)parentNodePositionVector, handleOrientation, undisplayedRelatedEntities);
        }

        private void SubscribeToModelEvents()
        {
            Model.RelationshipAdded += (o, e) => UpdateEnabledState();
            Model.RelationshipRemoved += (o, e) => UpdateEnabledState();
        }

        private void UpdateEnabledState()
        {
            if (AssociatedDiagramNode == null)
                return;

            IsEnabled = Diagram.GetUndisplayedRelatedEntities(AssociatedDiagramNode, RelatedEntitySpecification).Any();
        }

        //private static HandleOrientation CalculateHandleOrientation(RectRelativePointSpecification buttonLocation)
        //{
        //    switch (buttonLocation.Alignment.VerticalAlignment)
        //    {
        //        case VerticalAlignmentType.Top: return HandleOrientation.Bottom;
        //        case VerticalAlignmentType.Bottom: return HandleOrientation.Top;

        //        default: throw new NotImplementedException();
        //    }
        //}

        //private static Point CalculateAttachPoint(Rect rect, HandleOrientation handleOrientation)
        //{
        //    switch (handleOrientation)
        //    {
        //        case HandleOrientation.Top: return rect.GetRelativePoint(RectAlignment.BottomMiddle);
        //        case HandleOrientation.Bottom: return rect.GetRelativePoint(RectAlignment.TopMiddle);

        //        default: throw new NotImplementedException();
        //    }
        //}

    }
}
