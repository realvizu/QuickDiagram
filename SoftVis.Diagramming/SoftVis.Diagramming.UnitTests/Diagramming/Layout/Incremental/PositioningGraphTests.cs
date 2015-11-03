using System;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Diagramming.Layout.Incremental;
using FluentAssertions;
using Xunit;

namespace Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental
{
    public class PositioningGraphTests
    {
        private const int HorizontalGap = 10;
        private const int VerticalGap = 10;
        private readonly PositioningGraph _positioningGraph;

        public PositioningGraphTests()
        {
            _positioningGraph = new PositioningGraph(HorizontalGap, VerticalGap);
        }

        [Fact]
        public void AddFirstVertex_AddedAsFirstLayerFirstItem()
        {
            var vertex = CreateVertex("A");
            _positioningGraph.AddVertex(vertex);
            vertex.GetLayerIndex().Should().Be(0);
            vertex.GetIndexInLayer().Should().Be(0);
        }

        [Fact]
        public void AddSecondVertex_AddedAsFirstLayerNextItem()
        {
            var vertexA = CreateVertex("A");
            var vertexB = CreateVertex("B");

            _positioningGraph.AddVertex(vertexA);
            _positioningGraph.AddVertex(vertexB);

            vertexB.GetLayerIndex().Should().Be(0);
            vertexB.GetIndexInLayer().Should().Be(1);
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
                vertex.GetLayerIndex().Should().Be(1);
                vertex.GetIndexInLayer().Should().Be(0);
            }
            {
                _positioningGraph.AddEdge(CreateEdge("C", "O"));
                var vertex = GetVertex("C");
                vertex.GetLayerIndex().Should().Be(1);
                vertex.GetIndexInLayer().Should().Be(2);
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

            _positioningGraph.AddEdge(CreateEdge("B", "O"));
            var vertex = GetVertex("B");
            vertex.GetLayerIndex().Should().Be(1);
            vertex.GetIndexInLayer().Should().Be(1);
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
            vertex.GetLayerIndex().Should().Be(1);
            vertex.GetIndexInLayer().Should().Be(0);
        }

        private void SetUpPositioningGraph(params string[] pathSpecifications)
        {
            foreach (var pathSpecification in pathSpecifications)
            {
                foreach (var vertexName in GetVertexNames(pathSpecification))
                    _positioningGraph.AddVertex(CreateVertex(vertexName));

                foreach (var edgeSpecification in GetEdgeSpecifications(pathSpecification))
                    _positioningGraph.AddEdge(CreateEdge(edgeSpecification.SourceVertexName, edgeSpecification.TargetVertexName));
            }
        }

        private static IEnumerable<string> GetVertexNames(string pathSpecification)
        {
            return pathSpecification.Split(new[] { "<-" }, StringSplitOptions.None).ToArray();
        }

        private static IEnumerable<EdgeSpecification> GetEdgeSpecifications(string pathSpecification)
        {
            var vertexNames = pathSpecification.Split(new[] { "<-" }, StringSplitOptions.None).Reverse().ToArray();

            for (var i = 0; i < vertexNames.Length - 1; i++)
                yield return new EdgeSpecification(vertexNames[i], vertexNames[i+1]);
        }

        private PositioningVertexBase CreateVertex(string name)
        {
            return name.StartsWith("*")
                ? (PositioningVertexBase)new TestDummyPositioningVertex(_positioningGraph, int.Parse(name.Substring(1)), true)
                : new TestPositioningVertex(_positioningGraph, name, true);
        }

        private PositioningVertexBase GetVertex(string vertexName)
        {
            return _positioningGraph.Vertices.FirstOrDefault(i => i.Name == vertexName);
        }

        private PositioningEdge CreateEdge(string sourceVertexName, string targetVertexName)
        {
            var source = GetVertex(sourceVertexName);
            var target = GetVertex(targetVertexName);
            return new PositioningEdge(_positioningGraph, source, target, null);
        }
    }
}
