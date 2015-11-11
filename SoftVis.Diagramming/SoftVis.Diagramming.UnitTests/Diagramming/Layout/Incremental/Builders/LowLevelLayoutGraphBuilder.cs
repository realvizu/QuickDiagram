using Codartis.SoftVis.Diagramming.Layout.Incremental;
using Codartis.SoftVis.Diagramming.Layout.Incremental.Relative.Logic;

namespace Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental.Builders
{
    internal class LowLevelLayoutGraphBuilder : GraphBuilderBase<LayoutVertexBase, LayoutEdge, LowLevelLayoutGraph>
    {
        private readonly HighLevelLayoutGraphBuilder _highLevelLayoutGraphBuilder;

        public LowLevelLayoutGraphBuilder(HighLevelLayoutGraphBuilder highLevelLayoutGraphBuilder = null)
        {
            _highLevelLayoutGraphBuilder = highLevelLayoutGraphBuilder;
        }

        protected override LayoutVertexBase CreateGraphVertex(string name)
        {
            return _highLevelLayoutGraphBuilder?.GetVertex(name) ?? CreateLayoutVertex(name);
        }

        protected override LayoutEdge CreateGraphEdge(LayoutVertexBase source, LayoutVertexBase target)
        {
            return CreateLayoutEdge(source, target);
        }
    }
}