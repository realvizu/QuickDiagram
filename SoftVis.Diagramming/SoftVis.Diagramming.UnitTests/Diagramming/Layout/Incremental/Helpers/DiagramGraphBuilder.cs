using Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental.TestSubjects;

namespace Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental.Helpers
{
    internal class DiagramGraphBuilder : GraphBuilder<DiagramNode, DiagramConnector, DiagramGraph>
    {
        protected override DiagramNode CreateNewVertex(string name)
        {
            return new TestDiagramNode(name);
        }

        protected override DiagramConnector CreateNewEdge(DiagramNode source, DiagramNode target)
        {
            return new TestDiagramConnector(source, target);
        }
    }
}