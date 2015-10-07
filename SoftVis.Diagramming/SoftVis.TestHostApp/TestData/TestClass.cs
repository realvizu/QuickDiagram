using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.TestHostApp.TestData
{
    class TestClass : TestModelEntity
    {
        public TestClass(string name, int size)
            : base(name, size, ModelEntityType.Class)
        {
        }
    }
}
