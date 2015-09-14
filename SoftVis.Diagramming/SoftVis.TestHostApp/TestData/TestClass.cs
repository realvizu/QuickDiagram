using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.TestHostApp.TestData
{
    class TestClass : TestModelEntity
    {
        public TestClass(string name)
            : base(name, ModelEntityType.Class)
        {
        }
    }
}
