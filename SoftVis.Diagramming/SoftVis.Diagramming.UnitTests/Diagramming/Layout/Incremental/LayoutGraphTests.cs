using System;
using Codartis.SoftVis.Diagramming.Layout.Incremental;
using FluentAssertions;
using Xunit;

namespace Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental
{
    internal class LayoutGraphTests
    {
        private readonly TestLayoutGraphBuilder _testLayoutGraphBuilder;
        private LayoutGraph LayoutGraph => _testLayoutGraphBuilder.LayoutGraph;

        public LayoutGraphTests()
        {
            _testLayoutGraphBuilder = new TestLayoutGraphBuilder();
        }

        [Fact]
        public void AddEdge_DummyVertexWithMoreThanOneOutEdges_Throws()
        {
            _testLayoutGraphBuilder.SetUp("V1", "V2", "*1");
            {
                Action action = () => _testLayoutGraphBuilder.AddEdge("*1", "V1");
                action.ShouldNotThrow("because 1st out edge");
            }
            {
                Action action = () => _testLayoutGraphBuilder.AddEdge("*1", "V2");
                action.ShouldThrow<InvalidOperationException>("because 2nd out edge");
            }
        }

        [Fact]
        public void AddEdge_DummyVertexWithMoreThanOneInEdges_Throws()
        {
            _testLayoutGraphBuilder.SetUp("V1", "V2", "*1");
            {
                Action action = () => _testLayoutGraphBuilder.AddEdge("V1", "*1");
                action.ShouldNotThrow("because 1st in edge");
            }
            {
                Action action = () => _testLayoutGraphBuilder.AddEdge("V2", "*1");
                action.ShouldThrow<InvalidOperationException>("because 2nd in edge");
            }
        }

        [Fact]
        public void GetParents_Works()
        {
            _testLayoutGraphBuilder.SetUp(
                "P1<-C", 
                "*1<-C"
                );
            LayoutGraph.GetParents(_testLayoutGraphBuilder.GetVertex("C")).ShouldBeEquivalentTo(new[] {_testLayoutGraphBuilder.GetVertex("P1"), _testLayoutGraphBuilder.GetVertex("*1") });
        }

        [Fact]
        public void GetPrimaryParents_ChoosesFirstInNameOrder()
        {
            _testLayoutGraphBuilder.SetUp(
                "P1<-C", 
                "P2<-C"
                );
            LayoutGraph.GetPrimaryParent(_testLayoutGraphBuilder.GetVertex("C")).ShouldBeEquivalentTo(_testLayoutGraphBuilder.GetVertex("P1"));
        }

        [Fact]
        public void GetPrimaryParents_ChoosesCloserNonDummy()
        {
            _testLayoutGraphBuilder.SetUp(
                "P1<-*1<-*2<-C", 
                "P2<-*3<-C"
                );
            LayoutGraph.GetPrimaryParent(_testLayoutGraphBuilder.GetVertex("C")).ShouldBeEquivalentTo(_testLayoutGraphBuilder.GetVertex("*3"));
        }

        [Fact]
        public void GetPrimaryParents_ChoosesHigherPriority()
        {
            _testLayoutGraphBuilder.AddVertex("P2", 2);
            _testLayoutGraphBuilder.SetUp(
                "P1<-C", 
                "P2<-C"
                );
            LayoutGraph.GetPrimaryParent(_testLayoutGraphBuilder.GetVertex("C")).ShouldBeEquivalentTo(_testLayoutGraphBuilder.GetVertex("P2"));
        }

        [Fact]
        public void GetChildren_Works()
        {
            _testLayoutGraphBuilder.SetUp(
                "P<-C1", 
                "P<-*1"
                );
            LayoutGraph.GetChildren(_testLayoutGraphBuilder.GetVertex("P")).ShouldBeEquivalentTo(new[] {_testLayoutGraphBuilder.GetVertex("C1"), _testLayoutGraphBuilder.GetVertex("*1") });
        }

        [Fact]
        public void GetPrimaryChildren_Works()
        {
            _testLayoutGraphBuilder.AddVertex("P2", 2);
            _testLayoutGraphBuilder.SetUp(
                "P1<-C1", 
                "P1<-*1",
                "P1<-C2",
                "P2<-C2"
                );
            LayoutGraph.GetPrimaryChildren(_testLayoutGraphBuilder.GetVertex("P1")).ShouldBeEquivalentTo(new[] {_testLayoutGraphBuilder.GetVertex("C1"), _testLayoutGraphBuilder.GetVertex("*1") });
        }

        [Fact]
        public void GetPrimarySiblings_Works()
        {
            _testLayoutGraphBuilder.AddVertex("P1", 2);
            _testLayoutGraphBuilder.SetUp(
                "P1<-C1",
                "P1<-*1",
                "P1<-C2",
                "P2<-C2",
                "P2<-C3"
                );
            LayoutGraph.GetPrimarySiblings(_testLayoutGraphBuilder.GetVertex("C2")).ShouldBeEquivalentTo(new[] {_testLayoutGraphBuilder.GetVertex("C1"), _testLayoutGraphBuilder.GetVertex("*1") });
        }
    }
}
