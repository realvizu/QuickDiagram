using Codartis.SoftVis.Diagramming.Implementation;

namespace Codartis.SoftVis.Diagramming.UnitTests.TestSubjects
{
    internal sealed class TestDiagramConnector : DiagramConnector
    {
        public TestDiagramConnector(DiagramNode source, DiagramNode target) 
            : base(new TestModelRelationship(), source, target, ConnectorTypes.Generalization)
        {
        }
    }
}
