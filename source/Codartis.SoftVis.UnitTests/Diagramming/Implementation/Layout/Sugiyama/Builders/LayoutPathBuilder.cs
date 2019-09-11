using System.Linq;
using Codartis.SoftVis.Diagramming.Implementation.Layout.Sugiyama;
using Codartis.SoftVis.UnitTests.Diagramming.Implementation.Layout.Sugiyama.Helpers;

namespace Codartis.SoftVis.UnitTests.Diagramming.Implementation.Layout.Sugiyama.Builders
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
