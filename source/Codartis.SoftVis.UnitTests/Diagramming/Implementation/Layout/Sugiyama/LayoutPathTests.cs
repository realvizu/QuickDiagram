using System;
using System.Diagnostics.CodeAnalysis;
using Codartis.SoftVis.Diagramming.Implementation.Layout.Sugiyama;
using Codartis.SoftVis.Graphs;
using Codartis.SoftVis.UnitTests.Diagramming.Implementation.Layout.Sugiyama.Builders;
using FluentAssertions;
using Xunit;

namespace Codartis.SoftVis.UnitTests.Diagramming.Implementation.Layout.Sugiyama
{
    public class LayoutPathTests
    {
        private readonly LayoutPathBuilder _layoutPathBuilder;

        public LayoutPathTests()
        {
            _layoutPathBuilder = new LayoutPathBuilder();
        }

        [Fact]
        public void CheckInvariant_Fine()
        {
            {
                Action action = () => _layoutPathBuilder.CreateLayoutPath("A->B");
                action.Should().NotThrow();
            }
            {
                Action action = () => _layoutPathBuilder.CreateLayoutPath("A->*1->C");
                action.Should().NotThrow();
            }
            {
                Action action = () => _layoutPathBuilder.CreateLayoutPath("A->*1->*2->C");
                action.Should().NotThrow();
            }
        }

        [Fact]
        public void CheckInvariant_ViolatesInvariant_SourceAndTargetMustBeDiagramNodeLayoutVertex()
        {
            {
                Action action = () => _layoutPathBuilder.CreateLayoutPath("*1->B");
                action.Should().Throw<LayoutPathException>();
            }
            {
                Action action = () => _layoutPathBuilder.CreateLayoutPath("A->*1");
                action.Should().Throw<LayoutPathException>();
            }
        }

        [Fact]
        public void CheckInvariant_ViolatesInvariant_InterimVertexMustBeDummyLayoutVertex()
        {
            Action action = () => _layoutPathBuilder.CreateLayoutPath("A->B->C");
            action.Should().Throw<LayoutPathException>();
        }

        [Fact]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
        public void CheckInvariant_ViolatesBaseInvariant_EdgesDoNotFormAPath()
        {
            var edge1 = _layoutPathBuilder.CreateLayoutEdge("A->*1");
            var edge2 = _layoutPathBuilder.CreateLayoutEdge("*2->D");
            Action action = () => new LayoutPath(new[] { edge1, edge2 });
            action.Should().Throw<PathException>();
        }
    }
}