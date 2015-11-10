using System;
using Codartis.SoftVis.Diagramming.Layout.Incremental;
using Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental.Helpers;
using Codartis.SoftVis.Graphs;
using FluentAssertions;
using Xunit;

namespace Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental
{
    public class LayoutPathTests
    {
        [Fact]
        public void CheckInvariant_Fine()
        {
            {
                Action action = () => LayoutGraphFixtureHelper.CreatePath("A->B");
                action.ShouldNotThrow();
            }
            {
                Action action = () => LayoutGraphFixtureHelper.CreatePath("A->*1->C");
                action.ShouldNotThrow();
            }
            {
                Action action = () => LayoutGraphFixtureHelper.CreatePath("A->*1->*2->C");
                action.ShouldNotThrow();
            }
        }

        [Fact]
        public void CheckInvariant_ViolatesInvariant_SourceAndTargetMustBeDiagramNodeLayoutVertex()
        {
            {
                Action action = () => LayoutGraphFixtureHelper.CreatePath("*1->B");
                action.ShouldThrow<LayoutPathException>();
            }
            {
                Action action = () => LayoutGraphFixtureHelper.CreatePath("A->*1");
                action.ShouldThrow<LayoutPathException>();
            }
        }

        [Fact]
        public void CheckInvariant_ViolatesInvariant_InterimVertexMustBeDummyLayoutVertex()
        {
            Action action = () => LayoutGraphFixtureHelper.CreatePath("A->B->C");
            action.ShouldThrow<LayoutPathException>();
        }

        [Fact]
        public void CheckInvariant_ViolatesBaseInvariant_EdgesDoNotFormAPath()
        {
            var edge1 = LayoutGraphFixtureHelper.CreateEdge("A->*1");
            var edge2 = LayoutGraphFixtureHelper.CreateEdge("*2->D");
            Action action = () => new LayoutPath(new[] { edge1, edge2 });
            action.ShouldThrow<PathException>();
        }
    }
}
