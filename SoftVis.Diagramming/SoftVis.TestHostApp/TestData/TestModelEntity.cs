using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.TestHostApp.TestData
{
    internal abstract class TestModelEntity : IModelEntity
    {
        private readonly List<TestModelRelationship> _outgoingRelationships = new List<TestModelRelationship>();
        private readonly List<TestModelRelationship> _incomingRelationships = new List<TestModelRelationship>();

        public string Name { get; }
        public ModelEntityType Type { get; }

        protected TestModelEntity(string name, ModelEntityType type)
        {
            Name = name;
            Type = type;
        }

        public IEnumerable<IModelRelationship> OutgoingRelationships => _outgoingRelationships;
        public IEnumerable<IModelRelationship> IncomingRelationships => _incomingRelationships;

        public TestModelEntity BaseEntity
        {
            get { return _outgoingRelationships
                    .FirstOrDefault(i => i.Type == ModelRelationshipType.Generalization)
                    ?.TargetTestModelEntity; }
        }

        public void AddOutgoingRelationship(TestModelRelationship relationship)
        {
            _outgoingRelationships.Add(relationship);
        }

        public void AddIncomingRelationship(TestModelRelationship relationship)
        {
            _incomingRelationships.Add(relationship);
        }
    }
}
