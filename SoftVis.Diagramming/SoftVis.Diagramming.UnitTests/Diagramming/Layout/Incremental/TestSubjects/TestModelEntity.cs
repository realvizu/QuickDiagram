using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental.TestSubjects
{
    internal class TestModelEntity : ModelEntity
    {
        public TestModelEntity()
            :base(null, ModelEntityType.Class, null)
        {
        }

        public override int Priority => 0;
    }
}
