using Codartis.SoftVis.Diagramming.Layout.Incremental;
using Codartis.SoftVis.Diagramming.Layout.Incremental.Relative.Logic;

namespace Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental.Builders
{
    internal class QuasiProperLayoutGraphBuilder : GraphBuilderBase<LayoutVertexBase, GeneralLayoutEdge, QuasiProperLayoutGraph>
    {
        protected override LayoutVertexBase CreateGraphVertex(string name)
        {
            return CreateLayoutVertex(name);
        }

        protected override GeneralLayoutEdge CreateGraphEdge(LayoutVertexBase source, LayoutVertexBase target)
        {
            return CreateLayoutEdge(source, target);
        }
    }
}