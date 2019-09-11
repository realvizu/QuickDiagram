using Codartis.SoftVis.Diagramming.Implementation.Layout.Sugiyama;
using Codartis.SoftVis.Diagramming.Implementation.Layout.Sugiyama.Relative.Logic;

namespace Codartis.SoftVis.UnitTests.Diagramming.Implementation.Layout.Sugiyama.Builders
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