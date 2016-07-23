using Codartis.SoftVis.Diagramming.Implementation.Layout.Incremental;
using Codartis.SoftVis.Diagramming.Implementation.Layout.Incremental.Relative.Logic;

namespace Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental.Builders
{
    internal class QuasiProperLayoutGraphBuilder : GraphBuilderBase<LayoutVertexBase, GeneralLayoutEdge, QuasiProperLayoutGraph>
    {
        protected override LayoutVertexBase CreateVertex(string name)
        {
            return CreateLayoutVertex(name);
        }

        protected override GeneralLayoutEdge CreateEdge(LayoutVertexBase source, LayoutVertexBase target)
        {
            return CreateLayoutEdge(source, target);
        }
    }
}