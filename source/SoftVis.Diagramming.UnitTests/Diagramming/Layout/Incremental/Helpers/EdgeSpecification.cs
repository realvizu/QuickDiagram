using System;

namespace Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental.Helpers
{
    internal class EdgeSpecification : VertexList
    {
        public EdgeSpecification(string[] vertexNames)
            : base(vertexNames)
        {
            if (vertexNames.Length != 2)
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
