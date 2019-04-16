using Codartis.SoftVis.Diagramming.Layout.Incremental;
using Codartis.SoftVis.Diagramming.Layout.Incremental.Relative.Logic;

namespace Codartis.SoftVis.UnitTests.Diagramming.Layout.Incremental.Builders
{
    internal class QuasiProperLayoutGraphBuilder : GraphBuilderBase<LayoutVertexBase, GeneralLayoutEdge, QuasiProperLayoutGraph>
    {
        protected override LayoutVertexBase CreateVertex(string name, int priority = 1)
        {
            return CreateLayoutVertex(name, priority);
        }

        protected override GeneralLayoutEdge CreateEdge(LayoutVertexBase source, LayoutVertexBase target)
        {
            return CreateLayoutEdge(source, target);
        }
    }
}