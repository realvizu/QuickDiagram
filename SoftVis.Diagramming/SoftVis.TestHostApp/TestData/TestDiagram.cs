using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Implementation;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.TestHostApp.TestData
{
    internal class TestDiagram : Diagram
    {
        public List<List<IModelItem>> ModelItemGroups { get; }

        public TestDiagram(TestModel model)
            : base(model)
        {
            ModelItemGroups = model.ItemGroups.ToList();
        }

        public override IEnumerable<EntityRelationType> GetEntityRelationTypes()
        {
            foreach (var entityRelationType in base.GetEntityRelationTypes())
                yield return entityRelationType;

            yield return TestEntityRelationTypes.ImplementedInterfaces;
            yield return TestEntityRelationTypes.ImplementerTypes;
        }

        public override ConnectorType GetConnectorType(ModelRelationshipType type)
        {
            return type.Stereotype == TestModelRelationshipStereotypes.Implementation
                ? TestConnectorTypes.Implementation
                : ConnectorTypes.Generalization;
        }

        protected override Size2D CalculateDiagramNodeSize(IModelEntity modelEntity)
        {
            var height = 30;
            int nameAsInt;
            if (int.TryParse(modelEntity.Name, out nameAsInt))
                height = int.Parse(modelEntity.Name) % 4 * 5 + 25;

            return new Size2D(((TestModelEntity)modelEntity).Size, height);
        }
    }
}
