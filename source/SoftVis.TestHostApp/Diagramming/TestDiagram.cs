using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Implementation;
using Codartis.SoftVis.Modeling2;
using Codartis.SoftVis.TestHostApp.Modeling;

namespace Codartis.SoftVis.TestHostApp.Diagramming
{
    internal class TestDiagram : AutoArrangingDiagram
    {
        public TestDiagram(IModelBuilder notifyModelChanged)
            : base(notifyModelChanged)
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
