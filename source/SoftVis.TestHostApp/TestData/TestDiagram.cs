using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Implementation;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.TestHostApp.TestData
{
    internal class TestDiagram : AutoArrangingDiagram
    {
        public List<List<IModelItem>> ModelItemGroups { get; }

        public TestDiagram(TestModel model)
            : base(null)
        {
            ModelItemGroups = model.ItemGroups.ToList();
        }

        //public override IEnumerable<EntityRelationType> GetEntityRelationTypes()
        //{
        //    foreach (var entityRelationType in base.GetEntityRelationTypes())
        //        yield return entityRelationType;

        //    yield return TestEntityRelationTypes.ImplementedInterfaces;
        //    yield return TestEntityRelationTypes.ImplementerTypes;
        //}

        //public override ConnectorType GetConnectorType(ModelRelationshipType type)
        //{
        //    return type.Stereotype == TestModelRelationshipStereotypes.Implementation
        //        ? TestConnectorTypes.Implementation
        //        : ConnectorTypes.Generalization;
        //}
    }
}
