using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Events;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Util.UI;
using Codartis.SoftVis.Util.UI.Wpf;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Adds the ability to the viewport to follow the rect of some diagram nodes.
    /// </summary>
    public class AutoMoveViewportViewModel : ViewportCalculatorViewModel
    {
        private IDiagramNode[] _followedDiagramNodes;
        private TransitionSpeed _followDiagramNodesTransitionSpeed;

        public ViewportAutoMoveMode Mode { get; set; }

        public AutoMoveViewportViewModel(IReadOnlyModelStore modelStore, IReadOnlyDiagramStore diagramStore,
            double minZoom, double maxZoom, double initialZoom, ViewportAutoMoveMode mode = ViewportAutoMoveMode.Contain)
             : base(modelStore, diagramStore, minZoom, maxZoom, initialZoom)
        {
            Mode = mode;

            DiagramStore.DiagramChanged += OnDiagramChanged;
        }

        public override void Dispose()
        {
            base.Dispose();

            DiagramStore.DiagramChanged -= OnDiagramChanged;
        }

        public void FollowDiagramNodes(IEnumerable<IDiagramNode> diagramNodes, TransitionSpeed transitionSpeed)
        {
            _followedDiagramNodes = diagramNodes?.ToArray();
            _followDiagramNodesTransitionSpeed = transitionSpeed;
            MoveViewport();
        }

        public void StopFollowingDiagramNodes()
        {
            _followedDiagramNodes = null;
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

        private void OnDiagramChanged(DiagramEventBase diagramEvent)
        {
            EnsureUiThread(() => DispatchDiagramEvent(diagramEvent));
        }

        private void DispatchDiagramEvent(DiagramEventBase diagramEvent)
        {
            switch (diagramEvent)
            {
                case DiagramNodeSizeChangedEvent diagramNodeSizeChangedEvent:
                    FollowDiagramNode(diagramNodeSizeChangedEvent.DiagramNode);
                    break;
                case DiagramNodePositionChangedEvent diagramNodePositionChangedEvent:
                    FollowDiagramNode(diagramNodePositionChangedEvent.DiagramNode);
                    break;
            }
        }

        private void FollowDiagramNode(IDiagramNode diagramNode)
        {
            if (_followedDiagramNodes != null && _followedDiagramNodes.Contains(diagramNode))
                MoveViewport();
        }

        private void MoveViewport()
        {
            if (_followedDiagramNodes == null)
                return;

            var rect = _followedDiagramNodes.Where(i => i.IsRectDefined).Select(i => i.Rect).Union().ToWpf();
            if (rect.IsUndefined())
                return;

            switch (Mode)
            {
                case ViewportAutoMoveMode.Center:
                    ZoomToRect(rect, _followDiagramNodesTransitionSpeed);
                    break;
                case ViewportAutoMoveMode.Contain:
                    ContainRect(rect, _followDiagramNodesTransitionSpeed);
                    break;
            }
        }
    }
}
