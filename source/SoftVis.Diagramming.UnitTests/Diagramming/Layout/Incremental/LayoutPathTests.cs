﻿using System;
using Codartis.SoftVis.Diagramming.Layout.Incremental;
using Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental.Builders;
using Codartis.SoftVis.Graphs;
using FluentAssertions;
using Xunit;

namespace Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental
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
        public void CheckInvariant_ViolatesBaseInvariant_EdgesDoNotFormAPath()
        {
            var edge1 = _layoutPathBuilder.CreateLayoutEdge("A->*1");
            var edge2 = _layoutPathBuilder.CreateLayoutEdge("*2->D");
            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new LayoutPath(new[] { edge1, edge2 });
            action.Should().Throw<PathException>();
        }
    }
}
