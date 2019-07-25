using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Diagramming;
using FluentAssertions;

namespace Codartis.SoftVis.UnitTests.Diagramming
{
    public static class AssertionHelpers
    {
        public static void ShouldBeEquivalentById(this IEnumerable<IDiagramNode> nodes, params IDiagramNode[] expectedNodes)
        {
            nodes.Select(i => i.Id).Should().BeEquivalentTo(expectedNodes.Select(i => i.Id).ToArray());
        }

        public static void ShouldBeEquivalentById(this IEnumerable<IDiagramConnector> connectors, params IDiagramConnector[] expectedConnectors)
        {
            connectors.Select(i => i.Id).Should().BeEquivalentTo(expectedConnectors.Select(i => i.Id).ToArray());
        }
    }
}