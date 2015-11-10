using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental.TestSubjects
{
    internal class TestModelEntity : ModelEntity
    {
        public TestModelEntity(string name = null)
            :base(name, ModelEntityType.Class, null)
        {
        }

        public override int Priority => 0;
    }
}
