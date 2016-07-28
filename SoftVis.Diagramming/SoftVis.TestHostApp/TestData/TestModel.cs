using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Modeling.Implementation;

namespace Codartis.SoftVis.TestHostApp.TestData
{
    internal class TestModel : Model
    {
        private readonly List<List<IModelItem>> _modelItemGroups;

        public TestModel()
        {
            _modelItemGroups = new List<List<IModelItem>>();
            EndGroup();
        }

        public IEnumerable<List<IModelItem>> ItemGroups => _modelItemGroups;

        public override void AddEntity(ModelEntity testModelEntity)
        {
            base.AddEntity(testModelEntity);
            _modelItemGroups.Last().Add(testModelEntity);
        }

        public override void AddRelationship(ModelRelationship modelRelationship)
        {
            base.AddRelationship(modelRelationship);
            _modelItemGroups.Last().Add(modelRelationship);
        }

        public void EndGroup()
        {
            _modelItemGroups.Add(new List<IModelItem>());
        }
    }
}
