using Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental.TestSubjects;

namespace Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental.Builders
{
    internal class DiagramGraphBuilder : GraphBuilderBase<DiagramNode, DiagramConnector, DiagramGraph>
    {
        protected override DiagramNode CreateGraphVertex(string name)
        {
            return new TestDiagramNode(name);
        }

        protected override DiagramConnector CreateGraphEdge(DiagramNode source, DiagramNode target)
        {
            return new TestDiagramConnector(source, target);
        }
    }
}