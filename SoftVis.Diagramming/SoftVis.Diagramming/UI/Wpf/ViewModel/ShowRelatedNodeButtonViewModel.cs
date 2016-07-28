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
    internal class ShowRelatedNodeButtonViewModel : DiagramButtonViewModelBase
    {
        private readonly RelatedEntityButtonDescriptor _descriptor;
        private List<IModelEntity> _relatedEntities;
        private List<IModelEntity> _displayedRelatedEntities;
        private List<IModelEntity> _undisplayedRelatedEntities;

        public event EntitySelectorRequestedEventHandler EntitySelectorRequested;

        public ShowRelatedNodeButtonViewModel(IReadOnlyModel readOnlyModel, IDiagram diagram,
            double buttonRadius, RelatedEntityButtonDescriptor descriptor)
            : base(readOnlyModel, diagram, buttonRadius, descriptor.ButtonLocation)
        {
            _descriptor = descriptor;
            SubscribeToModelEvents();
        }

        public ConnectorType ConnectorType => _descriptor.ConnectorType;

        private RectRelativeLocation ButtonLocation => _descriptor.ButtonLocation;
        private RelatedEntitySpecification RelatedEntitySpecification => _descriptor.RelatedEntitySpecification;
        private DiagramNodeViewModel AssociatedDiagramNodeViewModel => (DiagramNodeViewModel)AssociatedDiagramShapeViewModel;
        private IDiagramNode AssociatedDiagramNode => AssociatedDiagramNodeViewModel?.DiagramNode;
        private IModelEntity AssociatedModelEntity => AssociatedDiagramNode?.ModelEntity;

        public override void AssociateWith(DiagramShapeViewModelBase diagramShapeViewModel)
        {
            base.AssociateWith(diagramShapeViewModel);
            UpdateDisplayedEntityInfo();
        }

        protected override void OnClick()
        {
            if (_undisplayedRelatedEntities.Count == 1)
            {
                Diagram.ShowItem(_undisplayedRelatedEntities.First());
            }
            else if (_undisplayedRelatedEntities.Count > 1)
            {
                RaiseEntitySelectorRequest();
            }
        }

        private void RaiseEntitySelectorRequest()
        {
            var handleOrientation = CalculateHandleOrientation(ButtonLocation);
            var parentNodePositionVector = (Vector) AssociatedDiagramNodeViewModel.Position;
            var rectInDiagramSpace = RelativeRect.Add(parentNodePositionVector);
            var attachPointInDiagramSpace = CalculateAttachPoint(rectInDiagramSpace, handleOrientation);
            EntitySelectorRequested?.Invoke(attachPointInDiagramSpace, handleOrientation, _undisplayedRelatedEntities);
        }

        private void UpdateDisplayedEntityInfo()
        {
            if (AssociatedDiagramNode == null)
                return;

            _relatedEntities = ReadOnlyModel.GetRelatedEntities(AssociatedModelEntity, RelatedEntitySpecification).ToList();
            _displayedRelatedEntities = _relatedEntities.Where(i => Diagram.Nodes.Any(j => j.ModelEntity == i)).ToList();
            _undisplayedRelatedEntities = _relatedEntities.Except(_displayedRelatedEntities).ToList();

            IsEnabled = _undisplayedRelatedEntities.Count > 0;
        }

        private void SubscribeToModelEvents()
        {
            ReadOnlyModel.EntityAdded += (o, e) => UpdateDisplayedEntityInfo();
            ReadOnlyModel.RelationshipAdded += (o, e) => UpdateDisplayedEntityInfo();
            ReadOnlyModel.EntityRemoved += (o, e) => UpdateDisplayedEntityInfo();
            ReadOnlyModel.RelationshipRemoved += (o, e) => UpdateDisplayedEntityInfo();
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
