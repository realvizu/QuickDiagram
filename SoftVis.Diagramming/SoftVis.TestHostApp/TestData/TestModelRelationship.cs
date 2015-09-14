using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.TestHostApp.TestData
{
    internal class TestModelRelationship : IModelRelationship
    {
        public TestModelEntity SourceTestModelEntity { get; }
        public TestModelEntity TargetTestModelEntity { get; }
        public ModelRelationshipType Type { get; }

        public TestModelRelationship(TestModelEntity source, TestModelEntity target, ModelRelationshipType type)
        {
            SourceTestModelEntity = source;
            TargetTestModelEntity = target;
            Type = type;
        }

        public IModelEntity Source => SourceTestModelEntity;
        public IModelEntity Target => TargetTestModelEntity;
    }
}
