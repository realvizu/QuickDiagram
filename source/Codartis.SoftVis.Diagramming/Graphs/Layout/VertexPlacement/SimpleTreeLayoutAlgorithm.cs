using System;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Geometry;
using QuickGraph;

namespace Codartis.SoftVis.Graphs.Layout.VertexPlacement
{
    /// <summary>
    /// Implements a graph layout algorithm that arranges the vertices into a top-down tree.
    /// The root (topmost vertex) is the one with no outbound edges.
    /// No edge routing.
    /// </summary>
    internal sealed class SimpleTreeLayoutAlgorithm<TVertex, TEdge> : IVertexPositioningAlgorithm<TVertex>
        where TVertex : class, ISized
        where TEdge : IEdge<TVertex>
    {
        public IDictionary<TVertex, Point2D> VertexCenters { get; private set; }

        private readonly IBidirectionalGraph<TVertex, TEdge> _graph;
        private readonly IList<Layer> _layers = new List<Layer>();
        private readonly IDictionary<TVertex, VertexData> _data = new Dictionary<TVertex, VertexData>();

        private const int LayerGap = 30;
        private const int VertexGap = 10;

        internal SimpleTreeLayoutAlgorithm(IBidirectionalGraph<TVertex, TEdge> graph)
        {
            _graph = graph;
        }

        /// <summary>
        /// Calculates the new positions of the graph's vertices. 
        /// </summary>
        public void Compute()
        {
            //first layout the vertices with 0 out-edge
            foreach (var vertex in _graph.Vertices.Where(v => _graph.OutDegree(v) == 0))
                CalculatePosition(vertex, null, 0);

            //then the others
            foreach (var vertex in _graph.Vertices)
                CalculatePosition(vertex, null, 0);

            VertexCenters = AssignPositions();
        }

        private double CalculatePosition(TVertex vertex, TVertex parent, int layerNumber)
        {
            if (_data.ContainsKey(vertex))
                return -1; //this vertex is already layed out

            while (layerNumber >= _layers.Count)
                _layers.Add(new Layer());

            var layer = _layers[layerNumber];
            var vertexSize = vertex.Size;
            var vertexData = new VertexData { Parent = parent };
            _data[vertex] = vertexData;

            layer.NextCenter += vertexSize.Width / 2.0;
            if (layerNumber > 0)
            {
                layer.NextCenter += _layers[layerNumber - 1].LastTranslate;
                _layers[layerNumber - 1].LastTranslate = 0;
            }
            layer.Size = Math.Max(layer.Size, vertexSize.Height + LayerGap);
            layer.Vertices.Add(vertex);

            if (_graph.InDegree(vertex) == 0)
            {
                vertexData.Center = layer.NextCenter;
            }
            else
            {
                var minPos = double.MaxValue;
                var maxPos = -double.MaxValue;

                foreach (var child in _graph.InEdges(vertex).Select(e => e.Source))
                {
                    var childPos = CalculatePosition(child, vertex, layerNumber + 1);
                    if (childPos >= 0)
                    {
                        minPos = Math.Min(minPos, childPos);
                        maxPos = Math.Max(maxPos, childPos);
                    }
                }
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (minPos != double.MaxValue)
                    vertexData.Center = (minPos + maxPos) / 2.0;
                else
                    vertexData.Center = layer.NextCenter;

                vertexData.Translate = Math.Max(layer.NextCenter - vertexData.Center, 0);

                layer.LastTranslate = vertexData.Translate;
                vertexData.Center += vertexData.Translate;
                layer.NextCenter = vertexData.Center;
            }

            layer.NextCenter += vertexSize.Width / 2.0 + VertexGap;

            return vertexData.Center;
        }

        private IDictionary<TVertex, Point2D> AssignPositions()
        {
            var newVertexPositions = new Dictionary<TVertex, Point2D>();

            double layerSize = 0;

            foreach (var layer in _layers)
            {
                foreach (var vertex in layer.Vertices)
                {
                    var size = vertex.Size;
                    var vertexData = _data[vertex];
                    if (vertexData.Parent != null)
                    {
                        vertexData.Center += _data[vertexData.Parent].Translate;
                        vertexData.Translate += _data[vertexData.Parent].Translate;
                    }

                    newVertexPositions[vertex] = new Point2D(vertexData.Center, layerSize + size.Height / 2.0);
                }
                layerSize += layer.Size;
            }

            return newVertexPositions;
        }

        private class Layer
        {
            internal double Size;
            internal double NextCenter;
            internal double LastTranslate;
            internal readonly IList<TVertex> Vertices = new List<TVertex>();
        }

        private class VertexData
        {
            internal TVertex Parent;
            internal double Translate;
            internal double Center;
        }
    }
}
