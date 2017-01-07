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

        public override IEnumerable<ModelEntityStereotype> GetModelEntityStereotypes()
        {
            foreach (var modelEntityStereotype in base.GetModelEntityStereotypes())
                yield return modelEntityStereotype;

            yield return TestModelEntityStereotypes.Interface;
        }

        public override IEnumerable<ModelRelationshipStereotype> GetModelRelationshipStereotypes()
        {
            foreach (var modelRelationshipStereotype in base.GetModelRelationshipStereotypes())
                yield return modelRelationshipStereotype;
            
            yield return TestModelRelationshipStereotypes.Implementation;
        }

        public override IModelEntity GetOrAddEntity(IModelEntity testModelEntity)
        {
            _modelItemGroups.Last().Add(testModelEntity);
            return base.GetOrAddEntity(testModelEntity);
        }

        public override ModelRelationship GetOrAddRelationship(ModelRelationship modelRelationship)
        {
            _modelItemGroups.Last().Add(modelRelationship);
            return base.GetOrAddRelationship(modelRelationship);
        }

        public void EndGroup()
        {
            _modelItemGroups.Add(new List<IModelItem>());
        }
    }
}
