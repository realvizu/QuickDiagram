using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.UI.Common;
using Codartis.SoftVis.UI.Extensibility;
using Codartis.SoftVis.UI.Geometry;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// A diagram button to choose related entities.
    /// </summary>
    internal class ShowRelatedNodeButtonViewModel : DiagramShapeButtonViewModelBase
    {
        private readonly RelatedEntityButtonDescriptor _descriptor;

        public event EntitySelectorRequestedEventHandler EntitySelectorRequested;

        public ShowRelatedNodeButtonViewModel(IReadOnlyModel model, IDiagram diagram,
            double buttonRadius, RelatedEntityButtonDescriptor descriptor)
            : base(model, diagram, buttonRadius, descriptor.ButtonLocation)
        {
            _descriptor = descriptor;
            SubscribeToModelEvents();
        }

        public ConnectorType ConnectorType => _descriptor.ConnectorType;

        private Rect RelativeRect => new Rect(RelativeTopLeft, Size);
        private RectRelativeLocation ButtonLocation => _descriptor.ButtonLocation;
        private RelatedEntitySpecification RelatedEntitySpecification => _descriptor.RelatedEntitySpecification;
        private DiagramNodeViewModel AssociatedDiagramNodeViewModel => (DiagramNodeViewModel)AssociatedDiagramShapeViewModel;
        private IDiagramNode AssociatedDiagramNode => AssociatedDiagramNodeViewModel?.DiagramNode;

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
            var handleOrientation = CalculateHandleOrientation(ButtonLocation);
            var parentNodePositionVector = (Vector)AssociatedDiagramNodeViewModel.Position;
            var rectInDiagramSpace = RelativeRect.Add(parentNodePositionVector);
            var attachPointInDiagramSpace = CalculateAttachPoint(rectInDiagramSpace, handleOrientation);
            EntitySelectorRequested?.Invoke(attachPointInDiagramSpace, handleOrientation, undisplayedRelatedEntities);
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

        private static HandleOrientation CalculateHandleOrientation(RectRelativeLocation buttonLocation)
        {
            switch (buttonLocation.Alignment.VerticalAlignment)
            {
                case VerticalAlignmentType.Top: return HandleOrientation.Bottom;
                case VerticalAlignmentType.Bottom: return HandleOrientation.Top;

                default: throw new NotImplementedException();
            }
        }

        private static Point CalculateAttachPoint(Rect rect, HandleOrientation handleOrientation)
        {
            switch (handleOrientation)
            {
                case HandleOrientation.Top: return rect.GetRelativePoint(RectAlignment.BottomMiddle);
                case HandleOrientation.Bottom: return rect.GetRelativePoint(RectAlignment.TopMiddle);

                default: throw new NotImplementedException();
            }
        }

    }
}
