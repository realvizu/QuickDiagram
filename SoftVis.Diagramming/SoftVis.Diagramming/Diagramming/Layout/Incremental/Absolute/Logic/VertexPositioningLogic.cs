using System;
using System.Diagnostics;
using System.Linq;
using Codartis.SoftVis.Common;
using Codartis.SoftVis.Diagramming.Layout.ActionGraph;
using Codartis.SoftVis.Diagramming.Layout.Incremental.Relative;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Graphs;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental.Absolute.Logic
{
    /// <summary>
    /// Implements the diagram node positioning logic of the incremental layout engine.
    /// TODO: move compact to another class?
    /// </summary>
    internal sealed class VertexPositioningLogic : AbsoluteLayoutActionEventSource
    {
        private readonly double _horizontalGap;
        private readonly double _verticalGap;
        private readonly IReadOnlyRelativeLayout _relativeLayout;
        private readonly LayoutActionGraph _layoutActionGraph;

        public VertexPositioningLogic(IReadOnlyRelativeLayout relativeLayout, double horizontalGap, double verticalGap)
        {
            _relativeLayout = relativeLayout;
            _horizontalGap = horizontalGap;
            _verticalGap = verticalGap;

            _layoutActionGraph = new LayoutActionGraph();
            LayoutActionExecuted += RecordLayoutActionIntoGraph;
        }

        private IReadOnlyQuasiProperLayoutGraph ProperLayeredLayoutGraph => _relativeLayout.ProperLayeredLayoutGraph;
        private IReadOnlyLayoutVertexLayers Layers => _relativeLayout.LayoutVertexLayers;

        public void PositionVertex(LayoutVertexBase layoutVertex, ILayoutAction causingAction)
        {
            _layoutActionGraph.Clear();

            var layoutAction = RaiseVertexLayoutAction("PositionVertex", layoutVertex, causingAction);

            var primaryParent = ProperLayeredLayoutGraph.GetPrimaryParent(layoutVertex);
            if (primaryParent == null)
                PositionRootVertex(layoutVertex, layoutAction);
            else
                PositionVertexUnderParent(layoutVertex, primaryParent, layoutAction);
        }

        public void Compact(ILayoutAction causingAction)
        {
            _layoutActionGraph.Clear();

            var layoutAction = RaiseLayoutAction("Compact", null, causingAction);

            CompactVertically(layoutAction);
            CompactHorizontally(layoutAction);
            AdjustVerticalPositions(layoutAction);
        }

        //public void CoverUpVertex(LayoutVertexBase layoutVertex, ILayoutAction causingAction)
        //{
        //    _layoutActionGraph.Clear();

        //    var layoutAction = RaiseVertexLayoutAction("CoverUpVertex", layoutVertex, causingAction);

        //    layoutVertex.IsFloating = true;
        //    var siblings = QuasiProperLayoutGraph.GetPrimarySiblings(layoutVertex).ToArray();

        //    CompactSiblings(siblings, layoutVertex, layoutAction);
        //}

        //private void CompactSiblings(LayoutVertexBase[] siblings, LayoutVertexBase removedVertex, ILayoutAction causingAction)
        //{
            // TODO: instead of this: skim through layers, find siblings with gap, compact as necessary
            // is it already done by compact?

            //if (!siblings.Any())
            //    return;

            //var layoutAction = RaiseVertexLayoutAction("CompactSiblings", removedVertex, causingAction);

            //var translateXAbs = (removedVertex.Width + _horizontalGap) / 2;

            //foreach (var sibling in siblings)
            //{
            //    var translateX = (sibling.Center.X < removedVertex.Center.X)
            //        ? translateXAbs
            //        : -translateXAbs;

            //    MoveTreeBy(sibling, translateX, layoutAction);
            //}
        //}

        private void CenterPrimaryParent(LayoutVertexBase layoutVertex, ILayoutAction causingAction)
        {
            var parentVertex = ProperLayeredLayoutGraph.GetPrimaryParent(layoutVertex);
            if (parentVertex == null)
                return;

            var parentTargetCenterX = ProperLayeredLayoutGraph.GetPlacedPrimaryChildren(parentVertex).GetRect().Center.X;
            if (parentTargetCenterX.IsEqualWithTolerance(parentVertex.Center.X))
                return;

            var layoutAction = RaiseVertexLayoutAction("CenterPrimaryParent", layoutVertex, causingAction);

            MoveVertexTo(parentVertex, parentTargetCenterX, layoutAction);
            CenterPrimaryParent(parentVertex, layoutAction);

            // TODO: how to place non-primary parents?
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
            var previousVertex = Layers.GetPreviousInLayer(rootVertex);
            var nextVertex = Layers.GetNextInLayer(rootVertex);

            return (previousVertex == null && nextVertex == null)
                ? 0
                : CalculateInsertionCenterXFromNeighbours(rootVertex, previousVertex, nextVertex);
        }

        private double GetNonRootVertexInsertionCenterX(LayoutVertexBase childVertex, LayoutVertexBase parentVertex)
        {
            if (!_relativeLayout.HasPlacedPrimarySiblingsInSameLayer(childVertex))
                return parentVertex.Center.X;

            var previousPlacedSibling = _relativeLayout.GetPreviousPlacedPrimarySiblingInSameLayer(childVertex);
            var nextPlacedSibling = _relativeLayout.GetNextPlacedPrimarySiblingInSameLayer(childVertex);

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
                foreach (var primaryChild in ProperLayeredLayoutGraph.GetPrimaryChildren(rootVertex))
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

            ProperLayeredLayoutGraph.ExecuteOnPrimaryDescendantVertices(rootVertex,
                i => MoveVertexBy(i, translateVectorX, layoutAction));

            CenterPrimaryParent(rootVertex, layoutAction);
        }

        private void FloatPrimaryTree(LayoutVertexBase vertex)
        {
            ProperLayeredLayoutGraph.ExecuteOnPrimaryDescendantVertices(vertex, i => i.IsFloating = true);
        }

        private void MoveVertexBy(LayoutVertexBase movingVertex, double translateVectorX, ILayoutAction causingAction)
        {
            if (movingVertex.Center == Point2D.Empty)
                return;

            MoveVertexTo(movingVertex, movingVertex.Center.X + translateVectorX, causingAction);
        }

        private void MoveVertexTo(LayoutVertexBase movingVertex, double newCenterX, ILayoutAction causingAction)
        {
            var newCenter = new Point2D(newCenterX, Layers.GetLayer(movingVertex).CenterY);
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
            var pushyVertexIndex = Layers.GetIndexInLayer(pushyVertex);
            foreach (var existingVertex in Layers.GetOtherPlacedVerticesInLayer(pushyVertex))
            {
                var existingVertexIndex = Layers.GetIndexInLayer(existingVertex);

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
                foreach (var layer in Layers)
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

                var leftVertexParent = ProperLayeredLayoutGraph.GetPrimaryParent(leftVertex);
                var rightVertexParent = ProperLayeredLayoutGraph.GetPrimaryParent(rightVertex);

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

            var rightVertexLeftmostChild = ProperLayeredLayoutGraph.GetPrimaryChildren(rightVertex)
                .OrderBy(i => i.Right).FirstOrDefault();
            if (rightVertexLeftmostChild == null)
                return gap;

            var previousVertex = Layers.GetPreviousInLayer(rightVertexLeftmostChild);
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

            foreach (var layoutVertex in ProperLayeredLayoutGraph.Vertices)
            {
                var newCenter = new Point2D(layoutVertex.Center.X, Layers.GetLayer(layoutVertex).CenterY);
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