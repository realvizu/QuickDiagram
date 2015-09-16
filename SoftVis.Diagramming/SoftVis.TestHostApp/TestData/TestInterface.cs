using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.TestHostApp.TestData
{
    class TestInterface : ModelEntity
    {
        public TestInterface(string name)
            : base(name, ModelEntityType.Class, TestModelEntityStereotype.Interface)
        {
        }
    }
}
