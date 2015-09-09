using System.Collections.Generic;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.TestHostApp.TestData
{
    abstract class TestModelEntity : IModelEntity
    {
        public string Name { get; }
        public ModelEntityType Type { get; }
        public TestModelEntity BaseEntity { get; }

        protected TestModelEntity(string name, ModelEntityType type, TestModelEntity baseEntity = null)
        {
            Name = name;
            Type = type;
            BaseEntity = baseEntity;
        }

        public IEnumerable<IModelRelationship> OutgoingRelationships
        {
            get
            {
                if (BaseEntity != null)
                    yield return new ModelRelationship(this, BaseEntity, ModelRelationshipType.Generalization);
            }
        }
    }
}
