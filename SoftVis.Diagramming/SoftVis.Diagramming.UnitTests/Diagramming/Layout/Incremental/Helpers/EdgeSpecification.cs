using System;
using System.Collections.Generic;
using System.Linq;

namespace Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental.Helpers
{
    internal class EdgeSpecification : VertexList
    {
        public EdgeSpecification(IEnumerable<string> vertexNames)
            : base(vertexNames)
        {
            if (vertexNames.Count() != 2)
                throw new ArgumentException("Edge specification must have exactly 2 vertices.");
        }

        public EdgeSpecification(string sourceVertexName, string targetVertexName)
            : this(new[] { sourceVertexName, targetVertexName })
        {
        }

        public string SourceVertexName => VertexNames[0];
        public string TargetVertexName => VertexNames[1];

        public static EdgeSpecification Parse(string pathString)
        {
            return new EdgeSpecification(InternalParse(pathString));
        }
    }
}
