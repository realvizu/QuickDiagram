using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.TestHostApp.TestData
{
    class TestClass : ModelEntity
    {
        public TestClass(string name)
            : base(name, ModelEntityType.Class)
        {
        }
    }
}
