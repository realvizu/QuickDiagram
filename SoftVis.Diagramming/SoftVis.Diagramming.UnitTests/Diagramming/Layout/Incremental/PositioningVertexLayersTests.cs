using System;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Diagramming.Layout.Incremental;
using FluentAssertions;
using Xunit;

namespace Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental
{
    public class PositioningVertexLayersTests
    {
        private const int HorizontalGap = 10;
        private const int VerticalGap = 10;
        private readonly PositioningGraph _positioningGraph;
        private readonly PositioningVertexLayers _layers;

        public PositioningVertexLayersTests()
        {
            _positioningGraph = new PositioningGraph();
            _layers = new PositioningVertexLayers(_positioningGraph, VerticalGap);
        }

        [Fact]
        public void AddFirstVertex_AddedAsFirstLayerFirstItem()
        {
            var vertex = CreateVertex("A");
            _positioningGraph.AddVertex(vertex);
            _layers.AddVertex(vertex);

            _layers.GetLayerIndex(vertex).Should().Be(0);
            _layers.GetIndexInLayer(vertex).Should().Be(0);
        }

        [Fact]
        public void AddSecondVertex_AddedAsFirstLayerNextItem()
        {
            var vertexA = CreateVertex("A");
            var vertexB = CreateVertex("B");

            _positioningGraph.AddVertex(vertexA);
            _layers.AddVertex(vertexA);
            _positioningGraph.AddVertex(vertexB);
            _layers.AddVertex(vertexB);

            _layers.GetLayerIndex(vertexB).Should().Be(0);
            _layers.GetIndexInLayer(vertexB).Should().Be(1);
        }

        [Fact]
        public void AddChildVertices_AddedNextToSiblingBasedOnNameOrder()
        {
            SetUpPositioningGraph(
                "A",
                "O<-B",
                "C"
            );

            {
                _positioningGraph.AddEdge(CreateEdge("A", "O"));
                var vertex = GetVertex("A");
                _layers.GetLayerIndex(vertex).Should().Be(1);
                _layers.GetIndexInLayer(vertex).Should().Be(0);
            }
            {
                _positioningGraph.AddEdge(CreateEdge("C", "O"));
                var vertex = GetVertex("C");
                _layers.GetLayerIndex(vertex).Should().Be(1);
                _layers.GetIndexInLayer(vertex).Should().Be(2);
            }
        }

        [Fact]
        public void AddChildVertices_AddedNextToSiblingBasedOnNameOrder_EvenIfSomeSiblingsAreDummy()
        {
            SetUpPositioningGraph(
                "O<-*1<-A",
                "O<-*2<-C",
                "B"
            );

            var edge = CreateEdge("B", "O");
            _positioningGraph.AddEdge(edge);
            _layers.AddEdge(edge);

            var vertex = GetVertex("B");
            _layers.GetLayerIndex(vertex).Should().Be(1);
            _layers.GetIndexInLayer(vertex).Should().Be(1);
        }

        [Fact]
        // TODO: fix this
        public void AddChildVertices_NoSiblings_AddedBasedOnParentOrder()
        {
            SetUpPositioningGraph(
                "O2<-A",
                "O1",
                "C"
            );

            _positioningGraph.AddEdge(CreateEdge("C", "O1"));
            var vertex = GetVertex("C");
            _layers.GetLayerIndex(vertex).Should().Be(1);
            _layers.GetIndexInLayer(vertex).Should().Be(0);
        }

        private void SetUpPositioningGraph(params string[] pathSpecifications)
        {
            foreach (var pathSpecification in pathSpecifications)
            {
                foreach (var vertexName in GetVertexNames(pathSpecification))
                {
                    var vertex = CreateVertex(vertexName);
                    _positioningGraph.AddVertex(vertex);
                    _layers.AddVertex(vertex);
                }

                foreach (var edgeSpecification in GetEdgeSpecifications(pathSpecification))
                {
                    var edge = CreateEdge(edgeSpecification.SourceVertexName, edgeSpecification.TargetVertexName);
                    _positioningGraph.AddEdge(edge);
                    _layers.AddEdge(edge);
                }
            }
        }

        private static IEnumerable<string> GetVertexNames(string pathSpecification)
        {
            return pathSpecification.Split(new[] { "<-" }, StringSplitOptions.None).ToArray();
        }

        private static IEnumerable<EdgeSpecification> GetEdgeSpecifications(string pathSpecification)
        {
            var vertexNames = pathSpecification.Split(new[] { "<-" }, StringSplitOptions.None).ToArray();

            for (var i = 0; i < vertexNames.Length - 1; i++)
                yield return new EdgeSpecification(vertexNames[i+1], vertexNames[i]);
        }

        private static PositioningVertexBase CreateVertex(string name)
        {
            return name.StartsWith("*")
                ? (PositioningVertexBase)new TestDummyPositioningVertex(int.Parse(name.Substring(1)), true)
                : new TestPositioningVertex(name, true);
        }

        private PositioningVertexBase GetVertex(string vertexName)
        {
            return _positioningGraph.Vertices.FirstOrDefault(i => i.Name == vertexName);
        }

        private PositioningEdge CreateEdge(string sourceVertexName, string targetVertexName)
        {
            var source = GetVertex(sourceVertexName);
            var target = GetVertex(targetVertexName);
            return new PositioningEdge(source, target, null);
        }
    }
}
