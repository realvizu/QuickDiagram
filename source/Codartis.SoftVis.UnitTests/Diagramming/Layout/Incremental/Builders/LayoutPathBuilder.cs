using System.Linq;
using Codartis.SoftVis.Diagramming.Layout.Nodes.Layered.Sugiyama;
using Codartis.SoftVis.UnitTests.Diagramming.Layout.Incremental.Helpers;

namespace Codartis.SoftVis.UnitTests.Diagramming.Layout.Incremental.Builders
{
    internal sealed class LayoutPathBuilder : BuilderBase
    {
        public LayoutPath CreateLayoutPath(string pathString)
        {
            var edgeSpecifications = PathSpecification.Parse(pathString).ToEdgeSpecifications();
            return new LayoutPath(edgeSpecifications.Select(CreateLayoutEdge));
        }

        public GeneralLayoutEdge CreateLayoutEdge(string edgeString)
        {
            var edgeSpecification = EdgeSpecification.Parse(edgeString);
            return CreateLayoutEdge(edgeSpecification);
        }
    }
}
