using System;
using System.Diagnostics;
using System.Linq;
using Codartis.SoftVis.Common;
using Codartis.SoftVis.Diagramming.Layout.ActionTracking.Implementation;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Graphs;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// Implements the vertex positioning logic of the incremental layout engine.
    /// </summary>
    internal sealed class VertexLayoutLogic : LayoutLogicBase
    {
        public VertexLayoutLogic(double horizontalGap, double verticalGap,
            LayoutGraph layoutGraph, LayoutActionGraph layoutActionGraph)
            : base(horizontalGap, verticalGap, layoutGraph, layoutActionGraph)
        {
        }

        public void AddVertex(LayoutVertexBase layoutVertex)
        {
            var layoutAction = RecordVertexAction("AddVertex", layoutVertex);

            LayoutGraph.AddVertex(layoutVertex);

            PositionVertex(layoutVertex, null, layoutAction);
        }

        public void RemoveVertex(LayoutVertexBase layoutVertex, LayoutAction previousAction = null)
        {
            var layoutAction = RecordVertexAction("RemoveVertex", layoutVertex, previousAction);

            var siblings = layoutVertex.GetPrimarySiblings().ToArray();

            LayoutGraph.RemoveVertex(layoutVertex);

            CompactSiblings(siblings, layoutVertex, layoutAction);

            Compact(layoutAction);
            AdjustVerticalPositions();
        }

        public void PositionVertex(LayoutVertexBase layoutVertex, LayoutVertexBase parentVertex, LayoutAction previousAction)
        {
            var layoutAction = RecordVertexAction("PositionVertex", layoutVertex, previousAction);

            var primaryParent = layoutVertex.GetPrimaryParent();
            if (primaryParent == null)
                PositionRootVertex(layoutVertex, layoutAction);
            else if (primaryParent == parentVertex)
                PositionVertexUnderParent(layoutVertex, primaryParent, layoutAction);

            Compact(layoutAction);
            AdjustVerticalPositions();
        }

        private void CompactSiblings(LayoutVertexBase[] siblings, LayoutVertexBase removedVertex, LayoutAction previousAction)
        {
            if (!siblings.Any())
                return;

            var layoutAction = RecordVertexAction("CompactSiblings", removedVertex, previousAction);

            var translateXAbs = (removedVertex.Width + HorizontalGap) / 2;

            foreach (var sibling in siblings)
            {
                var translateX = (sibling.Center.X < removedVertex.Center.X)
                    ? translateXAbs
                    : -translateXAbs;

                MoveTreeBy(sibling, translateX, layoutAction);
            }
        }

        private void PositionRootVertex(LayoutVertexBase rootVertex, LayoutAction previousAction)
        {
            var targetCenterX = GetRootVertexInsertionCenterX(rootVertex);
            MoveTreeTo(rootVertex, targetCenterX, previousAction);
        }

        private void PositionVertexUnderParent(LayoutVertexBase childVertex, LayoutVertexBase parentVertex, LayoutAction previousAction)
        {
            var targetCenterX = GetNonRootVertexInsertionCenterX(childVertex, parentVertex);
            MoveTreeTo(childVertex, targetCenterX, previousAction);
        }

        private double GetRootVertexInsertionCenterX(LayoutVertexBase rootVertex)
        {
            var previousVertex = rootVertex.GetPreviousInLayer();
            var nextVertex = rootVertex.GetNextInLayer();

            return (previousVertex == null && nextVertex == null)
                ? 0
                : CalculateInsertionCenterXFromNeighbours(rootVertex, previousVertex, nextVertex);
        }

        private double GetNonRootVertexInsertionCenterX(LayoutVertexBase childVertex, LayoutVertexBase parentVertex)
        {
            if (!childVertex.HasPrimarySiblingsInSameLayer())
                return parentVertex.Center.X;

            var previousSibling = childVertex.GetPreviousSiblingInLayer();
            var nextSibling = childVertex.GetNextSiblingInLayer();

            return CalculateInsertionCenterXFromNeighbours(childVertex, previousSibling, nextSibling);
        }

        private double CalculateInsertionCenterXFromNeighbours(LayoutVertexBase insertedVertex,
            LayoutVertexBase previousSibling, LayoutVertexBase nextSibling)
        {
            if (previousSibling == null && nextSibling == null)
                throw new ArgumentNullException(nameof(nextSibling) + " and " + nameof(previousSibling));

            return previousSibling == null
                ? nextSibling.Left - HorizontalGap - insertedVertex.Width / 2
                : previousSibling.Right + HorizontalGap + insertedVertex.Width / 2;
        }

        private LayoutAction MoveTreeTo(LayoutVertexBase rootVertex, double targetCenterX, LayoutAction previousAction)
        {
            return MoveTreeBy(rootVertex, targetCenterX - rootVertex.Center.X, previousAction);
        }

        private LayoutAction MoveTreeBy(LayoutVertexBase rootVertex, double translateVectorX, LayoutAction previousAction)
        {
            rootVertex.FloatPrimaryTree();

            var layoutAction = RecordVertexAction("MoveTree", rootVertex, translateVectorX, previousAction);

            rootVertex.ExecuteOnPrimaryDescendantVertices(i => MoveVertexBy(i, translateVectorX, layoutAction));

            CenterParents(rootVertex, layoutAction);

            return layoutAction;
        }

        private LayoutAction CenterParents(LayoutVertexBase layoutVertex, LayoutAction previousAction)
        {
            var parentVertex = layoutVertex.GetPrimaryParent();
            if (parentVertex == null)
                return null;

            var parentTargetCenterX = parentVertex.GetPrimaryPositionedChildren().GetRect().Center.X;
            if (parentTargetCenterX.IsEqualWithTolerance(parentVertex.Center.X))
                return null;

            var layoutAction = RecordVertexAction("CenterParent", layoutVertex, previousAction);

            MoveVertexTo(parentVertex, parentTargetCenterX, layoutAction);
            CenterParents(parentVertex, layoutAction);

            // TODO: how to place non-primary parents?

            return layoutAction;
        }

        private LayoutAction MoveVertexBy(LayoutVertexBase movingVertex, double translateVectorX, LayoutAction previousAction)
        {
            return MoveVertexTo(movingVertex, movingVertex.Center.X + translateVectorX, previousAction);
        }

        private LayoutAction MoveVertexTo(LayoutVertexBase movingVertex, double newCenterX, LayoutAction previousAction)
        {
            movingVertex.IsFloating = false;

            var oldCenter = movingVertex.Center;
            var newCenterY = movingVertex.GetLayer().CenterY;
            var newCenter = new Point2D(newCenterX, newCenterY);

            var layoutAction = RecordVertexMoveAction(movingVertex, oldCenter, newCenter, previousAction);

            if (!oldCenter.X.IsEqualWithTolerance(newCenterX) ||
                !oldCenter.Y.IsEqualWithTolerance(newCenterY))
            {
                movingVertex.Center = newCenter;
            }

            if (LayoutActionGraph.HasCycle())
                // TODO: find best empty place instead of push?
                Debug.WriteLine("***** Move cycle detected, pushing omitted.");
            else
                PushOtherVerticesFromTheWay(movingVertex, layoutAction);

            return layoutAction;
        }

        private void PushOtherVerticesFromTheWay(LayoutVertexBase pushyVertex, LayoutAction previousAction)
        {
            var pushyVertexIndex = pushyVertex.GetIndexInLayer();
            foreach (var existingVertex in pushyVertex.GetOtherPositionedVerticesInLayer())
            {
                var existingVertexIndex = existingVertex.GetIndexInLayer();

                if (existingVertexIndex < pushyVertexIndex)
                    EnsureVertexIsToTheLeft(existingVertex, pushyVertex, previousAction);
                else if (existingVertexIndex > pushyVertexIndex)
                    EnsureVertexIsToTheRight(existingVertex, pushyVertex, previousAction);
            }
        }

        private void EnsureVertexIsToTheRight(LayoutVertexBase rightVertex, LayoutVertexBase leftVertex, LayoutAction previousAction)
        {
            var vertexDistance = rightVertex.Left - leftVertex.Right;
            if (vertexDistance >= HorizontalGap)
                return;

            var translateVectorX = HorizontalGap - vertexDistance;

            var layoutAction = RecordVertexAction("PushVertexToTheRight", rightVertex, translateVectorX, previousAction);

            MoveTreeBy(rightVertex, translateVectorX, layoutAction);
        }

        private void EnsureVertexIsToTheLeft(LayoutVertexBase leftVertex, LayoutVertexBase rightVertex, LayoutAction previousAction)
        {
            var vertexDistance = rightVertex.Left - leftVertex.Right;
            if (vertexDistance >= HorizontalGap)
                return;

            var translateVectorX = vertexDistance - HorizontalGap;

            var layoutAction = RecordVertexAction("PushVertexToTheLeft", leftVertex, translateVectorX, previousAction);

            MoveTreeBy(leftVertex, translateVectorX, layoutAction);
        }

        public void Compact(LayoutAction previousAction)
        {
            CompactVertically(previousAction);
            CompactHorizontally(previousAction);
        }

        private void CompactVertically(LayoutAction previousAction)
        {
            //TODO: remove empty and dummy-only layers
        }

        private void CompactHorizontally(LayoutAction previousAction)
        {
            var gapFound = true;
            while (gapFound)
            {
                gapFound = false;
                foreach (var layer in LayoutGraph.Layers)
                {
                    gapFound = CompactLayer(layer, previousAction) || gapFound;
                }
            }
        }

        private bool CompactLayer(LayoutVertexLayer layer, LayoutAction previousAction)
        {
            var gapFound = false;
            for (var i = 1; i < layer.Count; i++)
            {
                var leftVertex = layer[i - 1];
                var rightVertex = layer[i];

                if (leftVertex.GetPrimaryParent() != rightVertex.GetPrimaryParent())
                    continue;

                var gap = DetermineGap(leftVertex, rightVertex);
                if (gap > HorizontalGap)
                {
                    gapFound = true;

                    var layoutAction = RecordAction("CompactLayer", layer.LayerIndex, previousAction);

                    RemoveGap(rightVertex, gap, layoutAction);
                }
            }
            return gapFound;
        }

        private double DetermineGap(LayoutVertexBase leftVertex, LayoutVertexBase rightVertex)
        {
            var gap = rightVertex.Left - leftVertex.Right;
            if (gap <= HorizontalGap)
                return gap;

            var rightVertexLeftmostChild = rightVertex.GetPrimaryChildren().OrderBy(i => i.Right).FirstOrDefault();
            if (rightVertexLeftmostChild == null)
                return gap;

            var previousVertex = rightVertexLeftmostChild.GetPreviousInLayer();
            if (previousVertex == null)
                return gap;

            var childrenGap = DetermineGap(previousVertex, rightVertexLeftmostChild);
            var result = Math.Min(childrenGap, gap);
            return result;
        }

        private void RemoveGap(LayoutVertexBase rightVertex, double gap, LayoutAction previousAction)
        {
            var layoutAction = RecordVertexAction("RemoveGap", rightVertex, gap, previousAction);

            var translate = -gap + HorizontalGap;
            MoveTreeBy(rightVertex, translate, layoutAction);
        }

        private void AdjustVerticalPositions()
        {
            foreach (var layoutVertex in LayoutGraph.Vertices)
                layoutVertex.RefreshVerticalPosition();
        }
    }
}