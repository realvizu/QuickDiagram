using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.TestHostApp.TestData
{
    internal class TestInterface : TestModelEntity
    {
        public TestInterface(string name, int size)
            : base(name, size, ModelEntityType.Class, TestModelEntityStereotype.Interface)
        {
        }

        public override int Priority => 1;
    }
}
