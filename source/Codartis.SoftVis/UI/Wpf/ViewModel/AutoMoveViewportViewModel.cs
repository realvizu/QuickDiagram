using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Diagramming.Definition.Events;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.Util.UI;
using Codartis.Util.UI.Wpf;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Adds the ability to the viewport to follow the rect of some diagram nodes.
    /// </summary>
    public class AutoMoveViewportViewModel : ViewportCalculatorViewModel
    {
        private ModelNodeId[] _followedNodeIds;
        private TransitionSpeed _followNodesTransitionSpeed;

        public ViewportAutoMoveMode Mode { get; set; }

        public AutoMoveViewportViewModel(IModelService modelService, IDiagramService diagramService,
            double minZoom, double maxZoom, double initialZoom, ViewportAutoMoveMode mode = ViewportAutoMoveMode.Contain)
             : base(modelService, diagramService, minZoom, maxZoom, initialZoom)
        {
            Mode = mode;

            DiagramService.DiagramChanged += OnDiagramChanged;
        }

        public override void Dispose()
        {
            base.Dispose();

            DiagramService.DiagramChanged -= OnDiagramChanged;
        }

        public void FollowDiagramNodes(IEnumerable<ModelNodeId> nodeIds, TransitionSpeed transitionSpeed)
        {
            _followedNodeIds = nodeIds?.ToArray();
            _followNodesTransitionSpeed = transitionSpeed;
            MoveViewport();
        }

        public void StopFollowingDiagramNodes()
        {
            _followedNodeIds = null;
        }

        public override void Pan(Vector panVectorInScreenSpace, TransitionSpeed transitionSpeed = TransitionSpeed.Fast)
        {
            StopFollowingDiagramNodes();
            base.Pan(panVectorInScreenSpace, transitionSpeed);
        }

        public override void Resize(Size sizeInScreenSpace, TransitionSpeed transitionSpeed = TransitionSpeed.Instant)
        {
            StopFollowingDiagramNodes();
            base.Resize(sizeInScreenSpace, transitionSpeed);
        }

        public override void ZoomToContent(TransitionSpeed transitionSpeed = TransitionSpeed.Medium)
        {
            StopFollowingDiagramNodes();
            base.ZoomToContent(transitionSpeed);
        }

        public override void ZoomWithCenterTo(double newLinearZoom, Point zoomCenterInScreenSpace, TransitionSpeed transitionSpeed = TransitionSpeed.Fast)
        {
            StopFollowingDiagramNodes();
            base.ZoomWithCenterTo(newLinearZoom, zoomCenterInScreenSpace, transitionSpeed);
        }

        private void OnDiagramChanged(DiagramEvent diagramEvent)
        {
            foreach (var change in diagramEvent.ShapeEvents)
                EnsureUiThread(() => DispatchDiagramEvent(change));
        }

        private void DispatchDiagramEvent(DiagramShapeEventBase diagramShapeEvent)
        {
            switch (diagramShapeEvent)
            {
                case DiagramNodeChangedEvent @event when @event.ChangedMember == DiagramNodeMember.Position:
                    FollowDiagramNode(@event.OldNode.Id);
                    break;
            }
        }

        private void FollowDiagramNode(ModelNodeId nodeId)
        {
            if (_followedNodeIds?.Contains(nodeId) == true)
                MoveViewport();
        }

        private void MoveViewport()
        {
            if (_followedNodeIds == null)
                return;

            var rect = DiagramService.LatestDiagram.GetRect(_followedNodeIds).ToWpf();
            if (rect.IsUndefined())
                return;

            switch (Mode)
            {
                case ViewportAutoMoveMode.Center:
                    ZoomToRect(rect, _followNodesTransitionSpeed);
                    break;
                case ViewportAutoMoveMode.Contain:
                    ContainRect(rect, _followNodesTransitionSpeed);
                    break;
            }
        }
    }
}
