using Codartis.SoftVis.Diagramming.Implementation;
using Codartis.SoftVis.Modeling2;

namespace Codartis.SoftVis.TestHostApp.Diagramming
{
    internal class TestDiagram : AutoArrangingDiagram
    {
        public TestDiagram(IModelBuilder modelBuilder, DiagramBuilder diagramBuilder)
            : base(modelBuilder, diagramBuilder)
        {
        }

        public void AddModelNode(IModelNode node)
        {
            ShowModelItem(node);
        }

        //public override IEnumerable<EntityRelationType> GetEntityRelationTypes()
        //{
        //    yield break;
        //}

        //public override ConnectorType GetConnectorType(ModelRelationshipType type)
        //{
        //    return ConnectorTypes.Generalization;
        //}
    }
}
