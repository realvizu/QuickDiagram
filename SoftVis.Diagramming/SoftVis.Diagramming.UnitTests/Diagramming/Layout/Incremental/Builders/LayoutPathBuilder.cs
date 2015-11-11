using System.Linq;
using Codartis.SoftVis.Diagramming.Layout.Incremental;

namespace Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental.Builders
{
    internal sealed class LayoutPathBuilder : BuilderBase
    {
        public LayoutPath CreateLayoutPath(string pathSpecification)
        {
            var edgeSpecifications = StringToEdgeSpecifications(pathSpecification);
            return new LayoutPath(edgeSpecifications.Select(CreateLayoutEdge));
        }

        public LayoutEdge CreateLayoutEdge(string edgeSpecification)
        {
            var edgeSpecifications = StringToEdgeSpecifications(edgeSpecification);
            return CreateLayoutEdge(edgeSpecifications.First());
        }
    }
}
