using System;
using Codartis.SoftVis.Diagramming.Implementation;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.Modeling.Implementation;
using Codartis.Util;
using FluentAssertions;
using Xunit;

namespace Codartis.SoftVis.UnitTests.Diagramming.Implementation
{
    public class DiagramNodeTests
    {
        [Fact]
        public void Rect_IsCalculatedCorrectly()
        {
            var modelNode = new ModelNode(ModelNodeId.Create(), "A", ModelNodeStereotype.Default);
            var diagramNode = new DiagramNode(
                modelNode,
                DateTime.Now,
                new Point2D(1, 2),
                new Size2D(3, 4),
                new Size2D(5, 6),
                Maybe<ModelNodeId>.Nothing);

            diagramNode.Rect.Should().Be(new Rect2D(1, 2, 1 + 5, 2 + 10));
        }
    }
}