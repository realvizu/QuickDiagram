using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Modeling.Implementation;

namespace Codartis.SoftVis.TestHostApp.TestData
{
    internal class BigTestModelBuilder
    {
        private readonly TestModel _testModel = new TestModel();

        public TestModel Create(int childCount, int levels)
        {
            var root = CreateAndAddClass("0");
            CreateChildren(root, childCount, 1, levels);
            return _testModel;
        }

        private TestClass CreateAndAddClass(string name)
        {
            var newEntity = new TestClass(name, 50);
            _testModel.AddEntity(newEntity);
            return newEntity;
        }

        private void CreateChildren(TestClass parent, int childCount, int currentLevel, int maxLevel)
        {
            if (currentLevel == maxLevel)
                return;

            for (var i = 0; i < childCount; i++)
            {
                var newEntity = CreateAndAddClass($"{parent.Name}-{i}");

                var newRelationship = new ModelRelationship(newEntity, parent, ModelRelationshipType.Generalization, ModelRelationshipStereotype.None);
                _testModel.AddRelationship(newRelationship);

                CreateChildren(newEntity, childCount, currentLevel + 1, maxLevel);
            }
        }
    }
}