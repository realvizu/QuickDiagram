using Codartis.SoftVis.Diagramming.Graph;
using Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental.TestSubjects;

namespace Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental.Builders
{
    internal class DiagramGraphBuilder : GraphBuilderBase<DiagramNode, DiagramConnector, DiagramGraph>
    {
        protected override DiagramNode CreateVertex(string name)
        {
            return new TestDiagramNode(name);
        }

        protected override DiagramConnector CreateEdge(DiagramNode source, DiagramNode target)
        {
            return new TestDiagramConnector(source, target);
        }
    }
}