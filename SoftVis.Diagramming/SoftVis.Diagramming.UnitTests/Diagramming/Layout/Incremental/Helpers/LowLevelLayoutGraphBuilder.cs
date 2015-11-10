using Codartis.SoftVis.Diagramming.Layout.Incremental;
using Codartis.SoftVis.Diagramming.Layout.Incremental.Relative.Logic;

namespace Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental.Helpers
{
    internal class LowLevelLayoutGraphBuilder : GraphBuilder<LayoutVertexBase, LayoutEdge, LowLevelLayoutGraph>
    {
        protected override LayoutVertexBase CreateNewVertex(string name)
        {
            return LayoutGraphFixtureHelper.CreateVertex(name);
        }

        protected override LayoutEdge CreateNewEdge(LayoutVertexBase source, LayoutVertexBase target)
        {
            return new LayoutEdge(source, target, null);
        }
    }
}