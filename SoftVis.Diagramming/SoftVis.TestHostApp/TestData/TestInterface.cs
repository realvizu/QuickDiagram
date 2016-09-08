using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.TestHostApp.TestData
{
    internal class TestInterface : TestModelEntity
    {
        public TestInterface(string name, int size, ModelOrigin origin)
            : base(name, size, ModelEntityClassifier.Class, TestModelEntityStereotypes.Interface, origin)
        {
        }

        public override int Priority => 1;
    }
}
