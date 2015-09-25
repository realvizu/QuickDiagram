using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using QuickGraph;

namespace Codartis.SoftVis.Diagramming.Graph.Layout.EfficientSugiyama
{
    internal class PositionCalculatorAlgorithm : IAlgorithm
    {
        private readonly SugiGraph _sugiGraph;
        private readonly EfficientSugiyamaLayoutParameters _layoutParameters;
        private readonly Layers _layers;
        private readonly IMutableBidirectionalGraph<Data, IEdge<Data>> _sparseCompactionGraph;
        private readonly List<SugiVertex> _isolatedVertices;
        private readonly IList<Edge<Data>>[] _sparseCompactionByLayerBackup;

        public Dictionary<IExtent, DiagramPoint> VertexCenters { get; private set; }

        internal PositionCalculatorAlgorithm(SugiGraph sugiGraph,
            EfficientSugiyamaLayoutParameters layoutParameters,
            Layers layers,
            IMutableBidirectionalGraph<Data, IEdge<Data>> sparseCompactionGraph,
            IList<Edge<Data>>[] sparseCompactionByLayerBackup,
            List<SugiVertex> isolatedVertices)
        {
            _sugiGraph = sugiGraph;
            _layoutParameters = layoutParameters;
            _layers = layers;
            _sparseCompactionGraph = sparseCompactionGraph;
            _sparseCompactionByLayerBackup = sparseCompactionByLayerBackup;
            _isolatedVertices = isolatedVertices;

            VertexCenters = new Dictionary<IExtent, DiagramPoint>();
        }

        private enum LeftRightMode : byte
        {
            Left = 0,
            Right = 1
        }

        private enum UpperLowerEdges : byte
        {
            Upper = 0,
            Lower = 1
        }

        public void Compute()
        {
            PutbackIsolatedVertices();
            CalculateLayerHeightsAndPositions();
            CalculateVerticalPositions();

            if (_layoutParameters.PositionMode < 0 || _layoutParameters.PositionMode == 0)
                CalculateHorizontalPositions(LeftRightMode.Left, UpperLowerEdges.Upper);
            if (_layoutParameters.PositionMode < 0 || _layoutParameters.PositionMode == 1)
                CalculateHorizontalPositions(LeftRightMode.Right, UpperLowerEdges.Upper);
            if (_layoutParameters.PositionMode < 0 || _layoutParameters.PositionMode == 2)
                CalculateHorizontalPositions(LeftRightMode.Left, UpperLowerEdges.Lower);
            if (_layoutParameters.PositionMode < 0 || _layoutParameters.PositionMode == 3)
                CalculateHorizontalPositions(LeftRightMode.Right, UpperLowerEdges.Lower);

            CalculateRealPositions();

            VertexCenters = GetVertexCenters(_sugiGraph);
        }

        private static Dictionary<IExtent, DiagramPoint> GetVertexCenters(SugiGraph sugiGraph)
        {
            var vertexCenters = new Dictionary<IExtent, DiagramPoint>();

            foreach (var sugiVertex in sugiGraph.Vertices.Where(i => i.Type == SugiVertexType.Original))
            {
                var newPosition = new DiagramPoint(sugiVertex.HorizontalPosition, sugiVertex.VerticalPosition);
                vertexCenters.Add(sugiVertex.OriginalVertex, newPosition);
            }

            return vertexCenters;
        }

        private void PutbackIsolatedVertices()
        {
            _sparseCompactionGraph.AddVertexRange(_isolatedVertices);
            _sugiGraph.AddVertexRange(_isolatedVertices);
            int layer = 0;
            SugiVertex prevIsolatedVertex = null;
            foreach (var isolatedVertex in _isolatedVertices)
            {
                var lastOnLayer = _sparseCompactionByLayerBackup[layer].LastOrDefault();
                _layers[layer].AddItem(isolatedVertex);
                isolatedVertex.LayerIndex = layer;
                isolatedVertex.Position = _layers[layer].Count - 1;
                if (lastOnLayer != null)
                {
                    var edge = new Edge<Data>(lastOnLayer.Target, isolatedVertex);
                    _sparseCompactionByLayerBackup[layer].Add(edge);
                    _sparseCompactionGraph.AddEdge(edge);
                }
                if (layer > 0 && prevIsolatedVertex != null)
                {
                    _sugiGraph.AddEdge(new SugiEdge(null, prevIsolatedVertex, isolatedVertex));
                }
                layer = (layer + 1) % _layers.Count;
                prevIsolatedVertex = isolatedVertex;
            }
        }

        private void CalculateLayerHeightsAndPositions()
        {
            double? previousLayerPosition = null;
            foreach (var layer in _layers)
            {
                layer.UpdateHight();
                layer.UpdatePosition(previousLayerPosition, _layoutParameters.LayerDistance);
                previousLayerPosition = layer.Position;
            }
        }

        private void CalculateRealPositions()
        {
            foreach (var vertex in _sugiGraph.Vertices)
            {
                Debug.WriteLine(string.Format("{0}:\t{1}\t{2}\t{3}\t{4}",
                    vertex.OriginalVertex,
                    vertex.HorizontalPositions[0],
                    vertex.HorizontalPositions[1],
                    vertex.HorizontalPositions[2],
                    vertex.HorizontalPositions[3]));
                if (_layoutParameters.PositionMode < 0)
                {
                    vertex.HorizontalPosition =
                        (vertex.HorizontalPositions[0] + vertex.HorizontalPositions[1]
                         + vertex.HorizontalPositions[2] + vertex.HorizontalPositions[3]) / 4.0;
                }
                else
                {
                    vertex.HorizontalPosition = vertex.HorizontalPositions[_layoutParameters.PositionMode];
                }
            }
        }

        private void CalculateVerticalPositions()
        {
            foreach (var vertex in _sugiGraph.Vertices)
                vertex.VerticalPosition = _layers[vertex].Position + 
                    (vertex.Size.Height <= 0 
                    ? _layers[vertex].Height 
                    : vertex.Size.Height) / 2.0;
        }

        /// <summary>
        /// Calculates the horizontal positions based on the selected modes.
        /// </summary>
        /// <param name="leftRightMode">Mode of the vertical alignment.</param>
        /// <param name="upperLowerEdges">Alignment based on which edges (upper or lower ones).</param>
        private void CalculateHorizontalPositions(LeftRightMode leftRightMode, UpperLowerEdges upperLowerEdges)
        {
            int modeIndex = (byte)upperLowerEdges * 2 + (byte)leftRightMode;
            InitializeRootsAndAligns(modeIndex);
            DoAlignment(modeIndex, leftRightMode, upperLowerEdges);
            WriteOutAlignment(modeIndex);
            InitializeSinksAndShifts(modeIndex);
            DoHorizontalCompaction(modeIndex, leftRightMode, upperLowerEdges);
        }

        private void WriteOutAlignment(int modeIndex)
        {
            Debug.WriteLine(string.Format("Alignment for {0}", modeIndex));
            foreach (var vertex in _sugiGraph.Vertices)
                Debug.WriteLine(string.Format("{0},{1},{2}: Root {3},{4},{5}\tAlign {6},{7},{8}",
                    vertex.OriginalVertex,
                    vertex.LayerIndex,
                    vertex.Type,
                    vertex.Roots[modeIndex].OriginalVertex,
                    vertex.Roots[modeIndex].LayerIndex,
                    vertex.Roots[modeIndex].Type,
                    vertex.Aligns[modeIndex].OriginalVertex,
                    vertex.Aligns[modeIndex].LayerIndex,
                    vertex.Aligns[modeIndex].Type));
        }

        private void InitializeSinksAndShifts(int modeIndex)
        {
            foreach (var vertex in _sugiGraph.Vertices)
            {
                vertex.Sinks[modeIndex] = vertex;
                vertex.Shifts[modeIndex] = double.PositiveInfinity;
                vertex.HorizontalPositions[modeIndex] = double.NaN;
            }
        }

        private void PlaceBlock(int modeIndex, LeftRightMode leftRightMode, UpperLowerEdges upperLowerEdges, SugiVertex v)
        {
            if (!double.IsNaN(v.HorizontalPositions[modeIndex]))
                return;

            double delta = _layoutParameters.VertexDistance;
            v.HorizontalPositions[modeIndex] = 0;
            Data w = v;
            do
            {
                SugiVertex wVertex = w as SugiVertex;
                Segment wSegment = w as Segment;
                if (_sparseCompactionGraph.ContainsVertex(w) &&
                    ((leftRightMode == LeftRightMode.Left && _sparseCompactionGraph.InDegree(w) > 0)
                      || (leftRightMode == LeftRightMode.Right && _sparseCompactionGraph.OutDegree(w) > 0)))
                {
                    var edges = leftRightMode == LeftRightMode.Left
                        ? _sparseCompactionGraph.InEdges(w)
                        : _sparseCompactionGraph.OutEdges(w);
                    foreach (var edge in edges)
                    {
                        SugiVertex u = null;
                        Data pred = leftRightMode == LeftRightMode.Left ? edge.Source : edge.Target;
                        if (pred is SugiVertex)
                            u = ((SugiVertex)pred).Roots[modeIndex];
                        else
                        {
                            var segment = (Segment)pred;
                            u = upperLowerEdges == UpperLowerEdges.Upper ? segment.PVertex.Roots[modeIndex] : segment.QVertex.Roots[modeIndex];
                        }
                        PlaceBlock(modeIndex, leftRightMode, upperLowerEdges, u);
                        if (v.Sinks[modeIndex] == v)
                            v.Sinks[modeIndex] = u.Sinks[modeIndex];
                        //var xDelta = delta + (v.Roots[modeIndex].BlockWidths[modeIndex] + u.BlockWidths[modeIndex]) / 2.0;
                        var xDelta = delta + ((wVertex != null ? wVertex.Size.Width : 0.0) + ((pred is SugiVertex) ? ((SugiVertex)pred).Size.Width : u.BlockWidths[modeIndex])) / 2.0;
                        if (v.Sinks[modeIndex] != u.Sinks[modeIndex])
                        {
                            var s = leftRightMode == LeftRightMode.Left
                                ? v.HorizontalPositions[modeIndex] - u.HorizontalPositions[modeIndex] - xDelta
                                : u.HorizontalPositions[modeIndex] - v.HorizontalPositions[modeIndex] - xDelta;

                            u.Sinks[modeIndex].Shifts[modeIndex] = leftRightMode == LeftRightMode.Left
                                ? Math.Min(u.Sinks[modeIndex].Shifts[modeIndex], s)
                                : Math.Max(u.Sinks[modeIndex].Shifts[modeIndex], s);
                        }
                        else
                        {
                            v.HorizontalPositions[modeIndex] =
                                leftRightMode == LeftRightMode.Left
                                    ? Math.Max(v.HorizontalPositions[modeIndex], u.HorizontalPositions[modeIndex] + xDelta)
                                    : Math.Min(v.HorizontalPositions[modeIndex], u.HorizontalPositions[modeIndex] - xDelta);
                        }
                    }
                }
                if (wSegment != null)
                    w = (upperLowerEdges == UpperLowerEdges.Upper) ? wSegment.QVertex : wSegment.PVertex;
                else if (wVertex.Type == SugiVertexType.PVertex && upperLowerEdges == UpperLowerEdges.Upper)
                    w = wVertex.Segment;
                else if (wVertex.Type == SugiVertexType.QVertex && upperLowerEdges == UpperLowerEdges.Lower)
                    w = wVertex.Segment;
                else
                    w = wVertex.Aligns[modeIndex];
            } while (w != v);
        }

        private void DoHorizontalCompaction(int modeIndex, LeftRightMode leftRightMode, UpperLowerEdges upperLowerEdges)
        {
            foreach (var vertex in _sugiGraph.Vertices)
            {
                if (vertex.Roots[modeIndex] == vertex)
                    PlaceBlock(modeIndex, leftRightMode, upperLowerEdges, vertex);
            }

            foreach (var vertex in _sugiGraph.Vertices)
            {
                vertex.HorizontalPositions[modeIndex] = vertex.Roots[modeIndex].HorizontalPositions[modeIndex];
                if (vertex.Roots[modeIndex].Sinks[modeIndex].Shifts[modeIndex] < double.PositiveInfinity &&
                    vertex.Roots[modeIndex] == vertex)
                    vertex.HorizontalPositions[modeIndex] += vertex.Roots[modeIndex].Sinks[modeIndex].Shifts[modeIndex];
            }
        }

        private void DoAlignment(int modeIndex, LeftRightMode leftRightMode, UpperLowerEdges upperLowerEdges)
        {
            int layerStart, layerEnd, layerStep;
            if (upperLowerEdges == UpperLowerEdges.Upper)
            {
                layerStart = 0;
                layerEnd = _layers.Count;
                layerStep = 1;
            }
            else
            {
                layerStart = _layers.Count - 1;
                layerEnd = -1;
                layerStep = -1;
            }
            for (int i = layerStart; i != layerEnd; i += layerStep)
            {
                int r = leftRightMode == LeftRightMode.Left ? int.MinValue : int.MaxValue;
                var layer = _layers[i];
                int vertexStart, vertexEnd, vertexStep;
                if (leftRightMode == LeftRightMode.Left)
                {
                    vertexStart = 0;
                    vertexEnd = layer.Count;
                    vertexStep = 1;
                }
                else
                {
                    vertexStart = layer.Count - 1;
                    vertexEnd = -1;
                    vertexStep = -1;
                }
                for (int j = vertexStart; j != vertexEnd; j += vertexStep)
                {
                    var vertex = layer[j];
                    if (vertex.Type == SugiVertexType.Original
                        || vertex.Type == SugiVertexType.RVertex
                        || (vertex.Type == SugiVertexType.PVertex && upperLowerEdges == UpperLowerEdges.Upper)
                        || (vertex.Type == SugiVertexType.QVertex && upperLowerEdges == UpperLowerEdges.Lower))
                    {
                        List<SugiEdge> neighbourEdges = null;
                        neighbourEdges = upperLowerEdges == UpperLowerEdges.Upper
                            ? _sugiGraph.InEdges(vertex).OrderBy(e => e.Source.Position).ToList()
                            : _sugiGraph.OutEdges(vertex).OrderBy(e => e.Target.Position).ToList();
                        if (neighbourEdges.Count <= 0)
                            continue;

                        int c1 = (int)Math.Floor((neighbourEdges.Count + 1) / 2.0) - 1;
                        int c2 = (int)Math.Ceiling((neighbourEdges.Count + 1) / 2.0) - 1;
                        int[] medians = null;
                        if (c1 == c2)
                        {
                            medians = new int[1] { c1 };
                        }
                        else
                        {
                            medians = leftRightMode == LeftRightMode.Left
                                ? new int[2] { c1, c2 }
                                : new int[2] { c2, c1 };
                        }
                        for (int m = 0; m < medians.Length; m++)
                        {
                            if (vertex.Aligns[modeIndex] != vertex)
                                continue;
                            var edge = neighbourEdges[medians[m]];
                            if (edge.Marked)
                                Debug.WriteLine("Edge marked: " + edge.Source.OriginalVertex + ", " + edge.Target.OriginalVertex);
                            var neighbour = edge.OtherVertex(vertex);
                            if (!edge.Marked &&
                                ((leftRightMode == LeftRightMode.Left && r < neighbour.Position)
                                    || (leftRightMode == LeftRightMode.Right && r > neighbour.Position)))
                            {
                                neighbour.Aligns[modeIndex] = vertex;
                                neighbour.BlockWidths[modeIndex] = Math.Max(neighbour.BlockWidths[modeIndex], vertex.Size.Width);
                                vertex.Roots[modeIndex] = neighbour.Roots[modeIndex];
                                vertex.Aligns[modeIndex] = vertex.Roots[modeIndex];
                                r = neighbour.Position;
                            }
                        }
                    }
                    else if (vertex.Type == SugiVertexType.PVertex /*&& upperLowerEdges == UpperLowerEdges.Lower*/)
                    {
                        //align the segment of the PVertex
                        vertex.Roots[modeIndex] = vertex.Segment.QVertex.Roots[modeIndex];
                        vertex.Aligns[modeIndex] = vertex.Roots[modeIndex];
                        r = vertex.Segment.Position;
                    }
                    else if (vertex.Type == SugiVertexType.QVertex /*&& upperLowerEdges == UpperLowerEdges.Upper*/)
                    {
                        //align the segment of the QVertex
                        vertex.Roots[modeIndex] = vertex.Segment.PVertex.Roots[modeIndex];
                        vertex.Aligns[modeIndex] = vertex.Roots[modeIndex];
                        r = vertex.Segment.Position;
                    }
                }
            }
        }

        private void InitializeRootsAndAligns(int modeIndex)
        {
            foreach (var vertex in _sugiGraph.Vertices)
            {
                vertex.Roots[modeIndex] = vertex;
                vertex.Aligns[modeIndex] = vertex;
                vertex.BlockWidths[modeIndex] = vertex.Size.Width;
            }
        }
    }
}
