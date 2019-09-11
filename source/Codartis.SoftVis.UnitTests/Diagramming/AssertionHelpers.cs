using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Modeling.Definition;
using FluentAssertions;

namespace Codartis.SoftVis.UnitTests.Diagramming
{
    public static class AssertionHelpers
    {
        public static void ShouldBeEquivalentById(this IEnumerable<IDiagramNode> nodes, params ModelNodeId[] expectedNodeIds)
        {
            nodes.Select(i => i.Id).Should().BeEquivalentTo(expectedNodeIds);
        }

        public static void ShouldBeEquivalentById(this IEnumerable<IDiagramConnector> connectors, params ModelRelationshipId[] expectedRelationshipIds)
        {
            connectors.Select(i => i.Id).Should().BeEquivalentTo(expectedRelationshipIds);
        }
    }
}