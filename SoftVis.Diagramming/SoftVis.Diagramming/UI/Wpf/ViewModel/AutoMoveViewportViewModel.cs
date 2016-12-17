using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Util.UI;
using Codartis.SoftVis.Util.UI.Wpf;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Adds the ability to the viewport to follow the rect of some diagram nodes.
    /// </summary>
    public class AutoMoveViewportViewModel : ViewportCalculatorViewModel, IDisposable
    {
        private IDiagramNode[] _followedDiagramNodes;
        private TransitionSpeed _followDiagramNodesTransitionSpeed;

        public ViewportAutoMoveMode Mode { get; set; }

        public AutoMoveViewportViewModel(IArrangedDiagram diagram, double minZoom, double maxZoom, double initialZoom,
            ViewportAutoMoveMode mode = ViewportAutoMoveMode.Contain)
             : base(diagram, minZoom, maxZoom, initialZoom)
        {
            Mode = mode;
            SubscribeToDiagramEvents();
        }

        public void Dispose()
        {
            UnsubscribeFromDiagramEvents();
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

        private void SubscribeToDiagramEvents()
        {
            Diagram.NodeTopLeftChanged += OnDiagramNodeTopLeftChanged;
            Diagram.NodeSizeChanged += OnDiagramNodeSizeChanged;
        }

        private void UnsubscribeFromDiagramEvents()
        {
            Diagram.NodeTopLeftChanged -= OnDiagramNodeTopLeftChanged;
            Diagram.NodeSizeChanged -= OnDiagramNodeSizeChanged;
        }

        private void OnDiagramNodeTopLeftChanged(IDiagramNode diagramNode, Point2D oldTopLeft, Point2D newTopLeft)
        {
            if (_followedDiagramNodes != null && _followedDiagramNodes.Contains(diagramNode))
                EnsureUiThread(MoveViewport);
        }

        private void OnDiagramNodeSizeChanged(IDiagramNode diagramNode, Size2D oldSize, Size2D newSize)
        {
            if (_followedDiagramNodes != null && _followedDiagramNodes.Contains(diagramNode))
                EnsureUiThread(MoveViewport);
        }

        private void MoveViewport()
        {
            if (_followedDiagramNodes == null)
                return;

            var rect = _followedDiagramNodes.Select(i => i.Rect).Where(i => i.IsDefined()).Union().ToWpf();
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
