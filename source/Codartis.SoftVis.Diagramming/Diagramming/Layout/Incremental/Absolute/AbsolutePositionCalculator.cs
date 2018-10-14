using System;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Diagramming.Layout.Incremental.Relative;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Util;
using MoreLinq;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental.Absolute
{
    /// <summary>
    /// Calculates vertex coordinates from relative layout info.
    /// </summary>
    /// <remarks>
    /// Postconditions:
    /// <para>if a vertex has children then its X pos is the center of the children's block</para>
    /// <para>if two vertices are in the same layer and v1 is before v2 in the relative layout
    /// then v1.Right + gap is smaller than v2.Left</para>
    /// </remarks>
    internal class AbsolutePositionCalculator
    {
        private readonly IReadOnlyRelativeLayout _relativeLayout;
        private readonly double _horizontalGap;
        private readonly double _verticalGap;

        private readonly Dictionary<IReadOnlyLayoutVertexLayer, double> _layerCenterYPositions;
        private readonly Dictionary<LayoutVertexBase, double> _vertexCenterXPositions;

        private AbsolutePositionCalculator(IReadOnlyRelativeLayout relativeLayout,
            double horizontalGap, double verticalGap)
        {
            _relativeLayout = relativeLayout;
            _horizontalGap = horizontalGap;
            _verticalGap = verticalGap;

            _layerCenterYPositions = new Dictionary<IReadOnlyLayoutVertexLayer, double>();
            _vertexCenterXPositions = new Dictionary<LayoutVertexBase, double>();
        }

        private IReadOnlyLayoutVertexLayers Layers => _relativeLayout.LayoutVertexLayers;
        private IReadOnlyQuasiProperLayoutGraph ProperLayoutGraph => _relativeLayout.ProperLayeredLayoutGraph;

        public static LayoutVertexToPointMap GetVertexCenters(IReadOnlyRelativeLayout relativeLayout,
            double horizontalGap, double verticalGap)
        {
            var calculator = new AbsolutePositionCalculator(relativeLayout, horizontalGap, verticalGap);
            return calculator.CalculateVertexCenters();
        }

        private LayoutVertexToPointMap CalculateVertexCenters()
        {
            CalculateXPositions();
            CalculateYPositions();

            var vertexCenters = new LayoutVertexToPointMap();
            foreach (var layer in Layers)
            {
                var yPos = _layerCenterYPositions[layer];
                foreach (var vertex in layer)
                {
                    var xPos = _vertexCenterXPositions[vertex];
                    var center = new Point2D(xPos, yPos);
                    vertexCenters.Set(vertex, center);
                }
            }

            CheckPostconditions(vertexCenters);
            return vertexCenters;
        }

        private void CalculateYPositions()
        {
            var previousBottom = -_verticalGap;
            foreach (var layer in Layers)
            {
                var height = layer.Any() ? layer.Select(i => i.Height).Max() : 0;
                var bottom = previousBottom + _verticalGap + height;

                _layerCenterYPositions[layer] = bottom - height / 2;
                previousBottom = bottom;
            }
        }

        private void CalculateXPositions()
        {
            foreach (var layer in Layers.Reverse())
            {
                var layerRight = double.MinValue;

                foreach (var vertex in layer)
                {
                    var vertexWidthHalf = vertex.Width / 2;
                    var children = ProperLayoutGraph.GetPrimaryChildren(vertex).ToList();

                    _vertexCenterXPositions[vertex] = children.Any()
                        ? GetMidPoint(children)
                        : Math.Max(layerRight, 0) + _horizontalGap + vertexWidthHalf;

                    RemoveGap(vertex, layerRight);
                    RemoveOverlap(vertex, layerRight);
                    layerRight = _vertexCenterXPositions[vertex] + vertexWidthHalf;
                }
            }
        }

        private double GetMidPoint(IEnumerable<LayoutVertexBase> children)
        {
            var left = double.MaxValue;
            var right = double.MinValue;

            foreach (var vertex in children)
            {
                var vertexCenterX = _vertexCenterXPositions[vertex];

                var vertexLeft = vertexCenterX - vertex.Width / 2;
                if (vertexLeft < left) left = vertexLeft;

                var vertexRight = vertexCenterX + vertex.Width / 2;
                if (vertexRight > right) right = vertexRight;
            }

            return (left + right) / 2;
        }

        private void RemoveGap(LayoutVertexBase vertex, double layerRight)
        {
            var gap = Left(vertex) - _horizontalGap - layerRight;
            if (gap > 0)
                ShiftLeftNeighbours(vertex, gap);
        }

        private void ShiftLeftNeighbours(LayoutVertexBase vertex, double shiftBy)
        {
            var layer = Layers.GetLayer(vertex);
            var indexInLayer = layer.IndexOf(vertex);

            var affectedVertices = layer.Take(indexInLayer).Reverse()
                .TakeWhile(i => !ProperLayoutGraph.HasPrimaryChildren(i)).ToArray();
            affectedVertices.ForEach(i => _vertexCenterXPositions[i] += shiftBy);
        }

        private void RemoveOverlap(LayoutVertexBase vertex, double layerRight)
        {
            var overlap = layerRight + _horizontalGap - Left(vertex);
            if (overlap > 0)
                ShiftWithRightNeighboursRecursive(vertex, overlap);
        }

        private void ShiftWithRightNeighboursRecursive(LayoutVertexBase vertex, double shiftBy)
        {
            var layer = Layers.GetLayer(vertex);
            var indexInLayer = layer.IndexOf(vertex);

            var affectedVertices = layer.Skip(indexInLayer).Where(_vertexCenterXPositions.ContainsKey).ToArray();
            affectedVertices.ForEach(i => _vertexCenterXPositions[i] += shiftBy);

            var nextLayerChildren = affectedVertices.SelectMany(ProperLayoutGraph.GetPrimaryChildren).ToArray();
            if (nextLayerChildren.Any())
                ShiftWithRightNeighboursRecursive(nextLayerChildren.MinBy(Left), shiftBy);
        }

        private double Left(LayoutVertexBase vertex)
        {
            return _vertexCenterXPositions[vertex] - vertex.Width / 2;
        }

        private void CheckPostconditions(LayoutVertexToPointMap vertexCenters)
        {
            CheckThatParentsAreCentered(vertexCenters);
            CheckCorrectOrderingAndNoOverlap(vertexCenters);
        }

        private void CheckThatParentsAreCentered(LayoutVertexToPointMap vertexCenters)
        {
            foreach (var vertex in ProperLayoutGraph.Vertices)
            {
                if (ProperLayoutGraph.HasPrimaryChildren(vertex))
                {
                    var childrenBlockRect = ProperLayoutGraph.GetPrimaryChildren(vertex)
                        .Select(i => Rect2D.CreateFromCenterAndSize(vertexCenters.Get(i), i.Size))
                        .Union();

                    if (!vertexCenters.Get(vertex).X.IsEqualWithTolerance(childrenBlockRect.Center.X))
                        throw new Exception($"{vertex} is not centered to its children.");
                }
            }
        }

        private void CheckCorrectOrderingAndNoOverlap(LayoutVertexToPointMap vertexCenters)
        {
            foreach (var layer in Layers)
            {
                for (var i = 0; i < layer.Count - 1; i++)
                {
                    var vertex1 = layer[i];
                    var vertex1HalfWidth = vertex1.Width / 2;
                    var vertex1Center = vertexCenters.Get(vertex1).X;

                    var vertex2 = layer[i + 1];
                    var vertex2HalfWidth = vertex2.Width / 2;
                    var vertex2Center = vertexCenters.Get(vertex2).X;

                    var vertex1RightSide = vertex1Center + vertex1HalfWidth;
                    var vertex2LeftSide = vertex2Center - vertex2HalfWidth;

                    if (vertex1RightSide > vertex2LeftSide)
                        throw new Exception($"{vertex1} overlaps {vertex2}.");
                }
            }
        }
    }
}
