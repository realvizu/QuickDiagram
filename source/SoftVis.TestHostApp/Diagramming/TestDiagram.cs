using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Implementation;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Modeling.Implementation;
using Codartis.SoftVis.Modeling2;
using Codartis.SoftVis.TestHostApp.Modeling;
using ModelOrigin = Codartis.SoftVis.Modeling.ModelOrigin;

namespace Codartis.SoftVis.TestHostApp.Diagramming
{
    internal class TestDiagram : AutoArrangingDiagram
    {
        public TestDiagram(TestModelBuilder model)
            : base(model)
        {
        }

        public void AddModelNode(IModelNode node)
        {
            ShowModelItem(new ModelEntity(node.FullName, "", "", ModelEntityClassifier.Class, ModelEntityStereotype.None, ModelOrigin.SourceCode));
        }

        public override IEnumerable<EntityRelationType> GetEntityRelationTypes()
        {
            yield break;
        }

        public override ConnectorType GetConnectorType(ModelRelationshipType type)
        {
            return ConnectorTypes.Generalization;
        }
    }
}
