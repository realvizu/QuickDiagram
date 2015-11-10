using System.Linq;
using Codartis.SoftVis.Diagramming.Layout.Incremental;
using Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental.TestSubjects;

namespace Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental.Helpers
{
    internal static class LayoutGraphFixtureHelper
    {
        public static LayoutVertexBase CreateVertex(string name, int priority = 1)
        {
            return name.StartsWith("*")
                ? (LayoutVertexBase) new TestDummyLayoutVertex(int.Parse(name.Substring(1)), true)
                : new TestLayoutVertex(name, true, priority);
        }

        public static LayoutPath CreatePath(string pathSpecification)
        {
            var edgeSpecifications = BuilderHelper.StringToEdgeSpecifications(pathSpecification);
            return new LayoutPath(edgeSpecifications.Select(CreateEdge));
        }

        public static LayoutEdge CreateEdge(string edgeSpecification)
        {
            return CreateEdge(BuilderHelper.StringToEdgeSpecifications(edgeSpecification).First());
        }

        private static LayoutEdge CreateEdge(EdgeSpecification edgeSpecification)
        {
            return CreateEdge(edgeSpecification.SourceVertexName, edgeSpecification.TargetVertexName);
        }

        private static LayoutEdge CreateEdge(string sourceVertexName, string targetVertexName)
        {
            var source = CreateVertex(sourceVertexName);
            var target = CreateVertex(targetVertexName);
            return new LayoutEdge(source, target, null);
        }

    }
}
