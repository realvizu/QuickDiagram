//using System.Linq;
//using Codartis.SoftVis.Diagramming.Layout.Incremental;
//using FluentAssertions;
//using Xunit;

//namespace Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental
//{
//    public class LayoutVertexLayersTests
//    {
//        private const int VerticalGap = 20;
//        private readonly TestLayoutGraphBuilder _testLayoutGraphBuilder;
//        private QuasiProperLayoutGraph QuasiProperLayoutGraph => _testLayoutGraphBuilder.QuasiProperLayoutGraph;
//        private readonly LayoutVertexLayers _layers;

//        public LayoutVertexLayersTests()
//        {
//            _testLayoutGraphBuilder = new TestLayoutGraphBuilder();
//            _layers = new LayoutVertexLayers(QuasiProperLayoutGraph);
//        }

//        [Fact]
//        public void AddVertex_PutInFirstLayerLastIndex()
//        {
//            var vertex1 = _testLayoutGraphBuilder.AddVertex("A1");
//            var vertex2 = _testLayoutGraphBuilder.AddVertex("A2");
//            _layers.AddVertex(vertex1);
//            _layers.AddVertex(vertex2);
//            _layers.Count().ShouldBeEquivalentTo(1);
//            _layers.First().ShouldBeEquivalentTo(new[] { vertex1, vertex2 }, options => options.WithStrictOrdering());
//        }

//        [Fact]
//        public void AddEdge_AddedToSiblingsInAlphabeticalOrder()
//        {
//            SetUp("P", "C1", "C2", "C3");

//            _layers.AddEdge(_testLayoutGraphBuilder.AddEdge("C2", "P"));
//            _layers.AddEdge(_testLayoutGraphBuilder.AddEdge("C3", "P"));
//            _layers.AddEdge(_testLayoutGraphBuilder.AddEdge("C1", "P"));

//            _layers.ToArray()[1].ShouldBeEquivalentTo(new[] { GetVertex("C1"), GetVertex("C2"), GetVertex("C3") },
//                options => options.WithStrictOrdering());
//        }

//        [Fact]
//        public void AddEdge_AddedToSiblingsInAlphabeticalOrder_DummyVerticesDoNotAffectResult()
//        {
//            SetUp(
//                "P<-*2<-C2",
//                "C1"
//                );

//            _layers.AddEdge(_testLayoutGraphBuilder.AddEdge("C1", "P"));

//            _layers.ToArray()[1].ShouldBeEquivalentTo(new[] { GetVertex("C1"), GetVertex("*2") },
//                options => options.WithStrictOrdering());
//        }

//        [Fact]
//        public void AddEdge_NonSiblingsAreOrderedByTheirParents()
//        {
//            SetUp(
//                "P3<-C3",
//                "P2",
//                "P1<-C1",
//                "C2"
//                );

//            _layers.AddEdge(_testLayoutGraphBuilder.AddEdge("C2", "P2"));

//            _layers.ToArray()[1].ShouldBeEquivalentTo(new[] { GetVertex("C3"), GetVertex("C2"), GetVertex("C1") },
//                options => options.WithStrictOrdering());
//        }

//        [Fact]
//        public void AddEdge_NonSiblingsAreOrderedByTheirParents_OtherNonParentVertexDoesNotAffectResult()
//        {
//            SetUp(
//                "P3<-C3",
//                "O1",
//                "P2",
//                "O2",
//                "P1<-C1",
//                "C2"
//                );

//            _layers.AddEdge(_testLayoutGraphBuilder.AddEdge("C2", "P2"));

//            _layers.ToArray()[1].ShouldBeEquivalentTo(new[] { GetVertex("C3"), GetVertex("C2"), GetVertex("C1") },
//                options => options.WithStrictOrdering());
//        }

//        [Fact]
//        public void AddEdge_NonSiblingsAreOrderedByTheirParents_DummyVerticesDoNotAffectResult()
//        {
//            SetUp(
//                "P3<-*3<-C3",
//                "P2",
//                "P1<-*1<-C1",
//                "C2"
//                );

//            _layers.AddEdge(_testLayoutGraphBuilder.AddEdge("C2", "P2"));

//            _layers.ToArray()[1].ShouldBeEquivalentTo(new[] { GetVertex("*3"), GetVertex("C2"), GetVertex("*1") },
//                options => options.WithStrictOrdering());
//        }

//        [Fact]
//        public void AddEdge_MovingToANewParent_OrderedBasedOnNewParent()
//        {
//            SetUp(
//                "P1<-C1",
//                "P2<-C2",
//                "P3"
//                );

//            _layers.ToArray()[1].ShouldBeEquivalentTo(new[] { GetVertex("C1"), GetVertex("C2") },
//                options => options.WithStrictOrdering());

//            _layers.RemoveEdge(_testLayoutGraphBuilder.RemoveEdge("C1", "P1"));

//            _layers.ToArray()[1].ShouldBeEquivalentTo(new[] { GetVertex("C1"), GetVertex("C2") },
//                options => options.WithStrictOrdering());

//            _layers.AddEdge(_testLayoutGraphBuilder.AddEdge("C1", "P3"));

//            _layers.ToArray()[1].ShouldBeEquivalentTo(new[] { GetVertex("C2"), GetVertex("C1")},
//                options => options.WithStrictOrdering());
//        }

//        private void SetUp(params string[] pathSpecifications)
//        {
//            foreach (var pathSpecification in pathSpecifications)
//            {
//                foreach (var vertexName in BuilderHelper.PathSpecificationToVertexNames(pathSpecification))
//                {
//                    var vertex = _testLayoutGraphBuilder.AddVertex(vertexName);
//                    _layers.AddVertex(vertex);
//                }
//                foreach (var edgeName in BuilderHelper.StringToEdgeSpecifications(pathSpecification))
//                {
//                    var edge = _testLayoutGraphBuilder.AddEdge(edgeName.SourceVertexName, edgeName.TargetVertexName);
//                    _layers.AddEdge(edge);
//                }
//            }
//        }

//        private LayoutVertexBase GetVertex(string vertexName)
//        {
//            return _testLayoutGraphBuilder.GetVertex(vertexName);
//        }
//    }
//}
