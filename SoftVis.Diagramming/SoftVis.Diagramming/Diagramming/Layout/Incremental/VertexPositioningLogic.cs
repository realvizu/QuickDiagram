using System;
using System.Diagnostics;
using System.Linq;
using Codartis.SoftVis.Common;
using Codartis.SoftVis.Diagramming.Layout.ActionTracking;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Graphs;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// Implements the diagram node positioning logic of the incremental layout engine.
    /// </summary>
    internal sealed class VertexPositioningLogic : IncrementalLayoutActionEventSource
    {
        private readonly double _horizontalGap;
        private readonly double _verticalGap;
        private readonly IReadOnlyLayoutGraph _layoutGraph;
        private readonly IReadOnlyLayoutVertexLayers _layers;
        private readonly LayoutActionGraph _layoutActionGraph;

        public VertexPositioningLogic(double horizontalGap, double verticalGap, IReadOnlyLayoutGraph layoutGraph,
            IReadOnlyLayoutVertexLayers layers)
        {
            _horizontalGap = horizontalGap;
            _verticalGap = verticalGap;
            _layoutGraph = layoutGraph;
            _layers = layers;

            _layoutActionGraph = new LayoutActionGraph();
            LayoutActionExecuted += RecordLayoutActionIntoGraph;
        }

        public void PositionVertex(LayoutVertexBase layoutVertex, ILayoutAction causingAction,
            LayoutVertexBase parentVertex = null)
        {
            _layoutActionGraph.Clear();

            var layoutAction = RaiseVertexLayoutAction("PositionVertex", layoutVertex, causingAction);

            var primaryParent = _layoutGraph.GetPrimaryParent(layoutVertex);
            if (primaryParent == null)
                PositionRootVertex(layoutVertex, layoutAction);
            else if (primaryParent == parentVertex)
                PositionVertexUnderParent(layoutVertex, primaryParent, layoutAction);
        }

        public void CoverUpVertex(LayoutVertexBase layoutVertex, ILayoutAction causingAction)
        {
            _layoutActionGraph.Clear();

            var layoutAction = RaiseVertexLayoutAction("CoverUpVertex", layoutVertex, causingAction);

            layoutVertex.IsFloating = true;
            var siblings = _layoutGraph.GetPrimarySiblings(layoutVertex).ToArray();

            CompactSiblings(siblings, layoutVertex, layoutAction);
        }

        public void CenterPrimaryParent(LayoutVertexBase layoutVertex, ILayoutAction causingAction)
        {
            var parentVertex = _layoutGraph.GetPrimaryParent(layoutVertex);
            if (parentVertex == null)
                return;

            var parentTargetCenterX = _layoutGraph.GetPlacedPrimaryChildren(parentVertex).GetRect().Center.X;
            if (parentTargetCenterX.IsEqualWithTolerance(parentVertex.Center.X))
                return;

            var layoutAction = RaiseVertexLayoutAction("CenterPrimaryParent", layoutVertex, causingAction);

            MoveVertexTo(parentVertex, parentTargetCenterX, layoutAction);
            CenterPrimaryParent(parentVertex, layoutAction);

            // TODO: how to place non-primary parents?
        }

        public void Compact(ILayoutAction causingAction)
        {
            _layoutActionGraph.Clear();

            var layoutAction = RaiseLayoutAction("Compact", null, causingAction);

            CompactVertically(layoutAction);
            CompactHorizontally(layoutAction);
            AdjustVerticalPositions(layoutAction);
        }

        private void CompactSiblings(LayoutVertexBase[] siblings, LayoutVertexBase removedVertex,
            ILayoutAction causingAction)
        {
            if (!siblings.Any())
                return;

            var layoutAction = RaiseVertexLayoutAction("CompactSiblings", removedVertex, causingAction);

            var translateXAbs = (removedVertex.Width + _horizontalGap) / 2;

            foreach (var sibling in siblings)
            {
                var translateX = (sibling.Center.X < removedVertex.Center.X)
                    ? translateXAbs
                    : -translateXAbs;

                MoveTreeBy(sibling, translateX, layoutAction);
            }
        }

        private void PositionRootVertex(LayoutVertexBase rootVertex, ILayoutAction causingAction)
        {
            var targetCenterX = GetRootVertexInsertionCenterX(rootVertex);
            MoveTreeTo(rootVertex, targetCenterX, causingAction);
        }

        private void PositionVertexUnderParent(LayoutVertexBase childVertex, LayoutVertexBase parentVertex,
            ILayoutAction causingAction)
        {
            var targetCenterX = GetNonRootVertexInsertionCenterX(childVertex, parentVertex);
            MoveTreeTo(childVertex, targetCenterX, causingAction);
        }

        private double GetRootVertexInsertionCenterX(LayoutVertexBase rootVertex)
        {
            var previousVertex = _layers.GetPreviousInLayer(rootVertex);
            var nextVertex = _layers.GetNextInLayer(rootVertex);

            return (previousVertex == null && nextVertex == null)
                ? 0
                : CalculateInsertionCenterXFromNeighbours(rootVertex, previousVertex, nextVertex);
        }

        private double GetNonRootVertexInsertionCenterX(LayoutVertexBase childVertex, LayoutVertexBase parentVertex)
        {
            if (!_layers.HasPlacedPrimarySiblingsInSameLayer(childVertex))
                return parentVertex.Center.X;

            var previousPlacedSibling = _layers.GetPreviousPlacedPrimarySiblingInSameLayer(childVertex);
            var nextPlacedSibling = _layers.GetNextPlacedPrimarySiblingInSameLayer(childVertex);

            return CalculateInsertionCenterXFromNeighbours(childVertex, previousPlacedSibling, nextPlacedSibling);
        }

        private double CalculateInsertionCenterXFromNeighbours(LayoutVertexBase insertedVertex,
            LayoutVertexBase previousSibling, LayoutVertexBase nextSibling)
        {
            if (previousSibling == null && nextSibling == null)
                throw new ArgumentNullException(nameof(nextSibling) + " and " + nameof(previousSibling));

            return previousSibling == null
                ? nextSibling.Left - _horizontalGap - insertedVertex.Width / 2
                : previousSibling.Right + _horizontalGap + insertedVertex.Width / 2;
        }

        private void MoveTreeTo(LayoutVertexBase rootVertex, double targetCenterX,
            ILayoutAction causingAction)
        {
            FloatPrimaryTree(rootVertex);

            var layoutAction = RaiseVertexLayoutAction("MoveTreeTo", rootVertex, targetCenterX, causingAction);

            if (rootVertex.Center != Point2D.Empty)
            {
                MoveTreeBy(rootVertex, targetCenterX - rootVertex.Center.X, layoutAction);
            }
            else
            {
                MoveVertexTo(rootVertex, targetCenterX, layoutAction);
                foreach (var primaryChild in _layoutGraph.GetPrimaryChildren(rootVertex))
                    MoveTreeTo(primaryChild, targetCenterX, layoutAction);
            }
        }

        private void MoveTreeBy(LayoutVertexBase rootVertex, double translateVectorX,
            ILayoutAction causingAction)
        {
            if (rootVertex.Center == Point2D.Empty)
                throw new InvalidOperationException("Tree cannot be moved relatively when the root's center is not yet calculated.");

            FloatPrimaryTree(rootVertex);

            var layoutAction = RaiseVertexLayoutAction("MoveTreeBy", rootVertex, translateVectorX, causingAction);

            _layoutGraph.ExecuteOnPrimaryDescendantVertices(rootVertex,
                i => MoveVertexBy(i, translateVectorX, layoutAction));

            CenterPrimaryParent(rootVertex, layoutAction);
        }

        private void FloatPrimaryTree(LayoutVertexBase vertex)
        {
            _layoutGraph.ExecuteOnPrimaryDescendantVertices(vertex, i => i.IsFloating = true);
        }

        private void MoveVertexBy(LayoutVertexBase movingVertex, double translateVectorX, ILayoutAction causingAction)
        {
            if (movingVertex.Center == Point2D.Empty)
                return;

            MoveVertexTo(movingVertex, movingVertex.Center.X + translateVectorX, causingAction);
        }

        private void MoveVertexTo(LayoutVertexBase movingVertex, double newCenterX, ILayoutAction causingAction)
        {
            var newCenter = new Point2D(newCenterX, _layers.GetLayer(movingVertex).CenterY);
            MoveVertexTo(movingVertex, newCenter, causingAction);
        }

        private void MoveVertexTo(LayoutVertexBase movingVertex, Point2D newCenter, ILayoutAction causingAction)
        {
            movingVertex.IsFloating = false;

            var oldCenter = movingVertex.Center;
            if (oldCenter.IsEqualWithTolerance(newCenter))
                return;

            movingVertex.Center = newCenter;
            var layoutAction = RaiseVertexMoveLayoutAction(movingVertex, oldCenter, newCenter, causingAction);

            if (_layoutActionGraph.HasCycle())
            {
                // TODO: find best empty place instead of push?
                Debug.WriteLine("***** Move cycle detected, pushing omitted.");
                Debugger.Break();
            }
            else
            {
                PushOtherVerticesFromTheWay(movingVertex, layoutAction);
            }
        }

        private void PushOtherVerticesFromTheWay(LayoutVertexBase pushyVertex, ILayoutAction causingAction)
        {
            var pushyVertexIndex = _layers.GetIndexInLayer(pushyVertex);
            foreach (var existingVertex in _layers.GetOtherPlacedVerticesInLayer(pushyVertex))
            {
                var existingVertexIndex = _layers.GetIndexInLayer(existingVertex);

                if (existingVertexIndex < pushyVertexIndex)
                    EnsureVertexIsToTheLeft(existingVertex, pushyVertex, causingAction);
                else if (existingVertexIndex > pushyVertexIndex)
                    EnsureVertexIsToTheRight(existingVertex, pushyVertex, causingAction);
            }
        }

        private void EnsureVertexIsToTheRight(LayoutVertexBase rightVertex, LayoutVertexBase leftVertex, ILayoutAction causingAction)
        {
            var vertexDistance = rightVertex.Left - leftVertex.Right;
            if (vertexDistance >= _horizontalGap)
                return;

            var translateVectorX = _horizontalGap - vertexDistance;

            var layoutAction = RaiseVertexLayoutAction("PushVertexToTheRight", rightVertex, translateVectorX, causingAction);

            MoveTreeBy(rightVertex, translateVectorX, layoutAction);
        }

        private void EnsureVertexIsToTheLeft(LayoutVertexBase leftVertex, LayoutVertexBase rightVertex, ILayoutAction causingAction)
        {
            var vertexDistance = rightVertex.Left - leftVertex.Right;
            if (vertexDistance >= _horizontalGap)
                return;

            var translateVectorX = vertexDistance - _horizontalGap;

            var layoutAction = RaiseVertexLayoutAction("PushVertexToTheLeft", leftVertex, translateVectorX, causingAction);

            MoveTreeBy(leftVertex, translateVectorX, layoutAction);
        }

        private void CompactVertically(ILayoutAction causingAction)
        {
            //TODO: remove empty and dummy-only layers
        }

        private void CompactHorizontally(ILayoutAction causingAction)
        {
            var gapFound = true;
            while (gapFound)
            {
                gapFound = false;
                foreach (var layer in _layers)
                {
                    gapFound = CompactLayer(layer, causingAction) || gapFound;
                }
            }
        }

        private bool CompactLayer(IReadOnlyLayoutVertexLayer layer, ILayoutAction causingAction)
        {
            var gapFound = false;
            for (var i = 1; i < layer.Count; i++)
            {
                var leftVertex = layer[i - 1];
                var rightVertex = layer[i];

                var leftVertexParent = _layoutGraph.GetPrimaryParent(leftVertex);
                var rightVertexParent = _layoutGraph.GetPrimaryParent(rightVertex);

                if (leftVertexParent != rightVertexParent)
                    continue;

                var gap = DetermineGap(leftVertex, rightVertex);
                if (gap > _horizontalGap)
                {
                    gapFound = true;

                    var layoutAction = RaiseLayoutAction("CompactLayer", layer.LayerIndex, causingAction);

                    RemoveGap(rightVertex, gap, layoutAction);
                }
            }
            return gapFound;
        }

        private double DetermineGap(LayoutVertexBase leftVertex, LayoutVertexBase rightVertex)
        {
            var gap = rightVertex.Left - leftVertex.Right;
            if (gap <= _horizontalGap)
                return gap;

            var rightVertexLeftmostChild = _layoutGraph.GetPrimaryChildren(rightVertex)
                .OrderBy(i => i.Right).FirstOrDefault();
            if (rightVertexLeftmostChild == null)
                return gap;

            var previousVertex = _layers.GetPreviousInLayer(rightVertexLeftmostChild);
            if (previousVertex == null)
                return gap;

            var childrenGap = DetermineGap(previousVertex, rightVertexLeftmostChild);
            var result = Math.Min(childrenGap, gap);
            return result;
        }

        private void RemoveGap(LayoutVertexBase rightVertex, double gap, ILayoutAction causingAction)
        {
            var layoutAction = RaiseVertexLayoutAction("RemoveGap", rightVertex, gap, causingAction);

            var translate = -gap + _horizontalGap;
            MoveTreeBy(rightVertex, translate, layoutAction);
        }

        private void AdjustVerticalPositions(ILayoutAction causingAction)
        {
            var layoutAction = RaiseLayoutAction("AdjustVerticalPositions", null, causingAction);

            foreach (var layoutVertex in _layoutGraph.Vertices)
            {
                var newCenter = new Point2D(layoutVertex.Center.X, _layers.GetLayer(layoutVertex).CenterY);
                MoveVertexTo(layoutVertex, newCenter, layoutAction);
            }
        }

        private void RecordLayoutActionIntoGraph(object source, ILayoutAction layoutAction)
        {
            if (!_layoutActionGraph.ContainsVertex(layoutAction))
                _layoutActionGraph.AddVertex(layoutAction);

            if (layoutAction.CausingLayoutAction != null &&
                _layoutActionGraph.ContainsVertex(layoutAction.CausingLayoutAction))
            {
                var edge = new LayoutActionEdge(layoutAction.CausingLayoutAction, layoutAction);
                _layoutActionGraph.AddEdge(edge);
            }
        }
    }
}