using System;
using System.Collections.Generic;
using System.Linq;

namespace Codartis.SoftVis.Diagramming.Shapes.Graph.Layout
{
    /// <summary>
    /// Implements a graph layout algorithm that arranges the vertices into a top-down tree.
    /// The root (topmost vertex) is the one with no outbound edges.
    /// No edge routing.
    /// </summary>
    internal sealed class SimpleTreeLayoutAlgorithm
    {
        private readonly DiagramGraph _graph;
        private readonly IList<Layer> _layers = new List<Layer>();
        private readonly IDictionary<DiagramNode, VertexData> _data = new Dictionary<DiagramNode, VertexData>();

        private const int LayerGap = 30;
        private const int VertexGap = 10;

        internal SimpleTreeLayoutAlgorithm(DiagramGraph graph)
        {
            _graph = graph;
        }

        /// <summary>
        /// Calculates the new positions of the graph's vertices. 
        /// </summary>
        /// <returns>A dictionary of the (vertex, position) items.</returns>
        public IDictionary<DiagramNode, DiagramPoint> ComputeNewVertexPositions()
        {
            //first layout the vertices with 0 out-edge
            foreach (var vertex in _graph.Vertices.Where(v => _graph.OutDegree(v) == 0))
                CalculatePosition(vertex, null, 0);

            //then the others
            foreach (var vertex in _graph.Vertices)
                CalculatePosition(vertex, null, 0);

            var newVertexPositions = AssignPositions();
            return newVertexPositions;
        }

        private double CalculatePosition(DiagramNode vertex, DiagramNode parent, int layerNumber)
        {
            if (_data.ContainsKey(vertex))
                return -1; //this vertex is already layed out

            while (layerNumber >= _layers.Count)
                _layers.Add(new Layer());

            var layer = _layers[layerNumber];
            var vertexSize = vertex.Size;
            var vertexData = new VertexData { Parent = parent };
            _data[vertex] = vertexData;

            layer.NextPosition += vertexSize.Width / 2.0;
            if (layerNumber > 0)
            {
                layer.NextPosition += _layers[layerNumber - 1].LastTranslate;
                _layers[layerNumber - 1].LastTranslate = 0;
            }
            layer.Size = Math.Max(layer.Size, vertexSize.Height + LayerGap);
            layer.Vertices.Add(vertex);

            if (_graph.InDegree(vertex) == 0)
            {
                vertexData.Position = layer.NextPosition;
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
                    vertexData.Position = (minPos + maxPos) / 2.0;
                else
                    vertexData.Position = layer.NextPosition;

                vertexData.Translate = Math.Max(layer.NextPosition - vertexData.Position, 0);

                layer.LastTranslate = vertexData.Translate;
                vertexData.Position += vertexData.Translate;
                layer.NextPosition = vertexData.Position;
            }

            layer.NextPosition += vertexSize.Width / 2.0 + VertexGap;

            return vertexData.Position;
        }

        private IDictionary<DiagramNode, DiagramPoint> AssignPositions()
        {
            var newVertexPositions = new Dictionary<DiagramNode, DiagramPoint>();

            double layerSize = 0;

            foreach (var layer in _layers)
            {
                foreach (var vertex in layer.Vertices)
                {
                    var size = vertex.Size;
                    var vertexData = _data[vertex];
                    if (vertexData.Parent != null)
                    {
                        vertexData.Position += _data[vertexData.Parent].Translate;
                        vertexData.Translate += _data[vertexData.Parent].Translate;
                    }

                    newVertexPositions[vertex] = new DiagramPoint(vertexData.Position, layerSize + size.Height / 2.0);
                }
                layerSize += layer.Size;
            }

            return newVertexPositions;
        }

        private class Layer
        {
            internal double Size;
            internal double NextPosition;
            internal double LastTranslate;
            internal readonly IList<DiagramNode> Vertices = new List<DiagramNode>();
        }

        private class VertexData
        {
            internal DiagramNode Parent;
            internal double Translate;
            internal double Position;
        }
    }
}
