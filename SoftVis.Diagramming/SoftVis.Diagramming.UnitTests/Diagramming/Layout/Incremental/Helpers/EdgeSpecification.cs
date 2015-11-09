namespace Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental.Helpers
{
    internal struct EdgeSpecification
    {
        public readonly string SourceVertexName;
        public readonly string TargetVertexName;

        public EdgeSpecification(string sourceVertexName, string targetVertexName)
        {
            SourceVertexName = sourceVertexName;
            TargetVertexName = targetVertexName;
        }
    }
}
