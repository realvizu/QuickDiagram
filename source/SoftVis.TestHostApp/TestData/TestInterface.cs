using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.TestHostApp.TestData
{
    internal class TestInterface : TestModelEntity
    {
        public TestInterface(string name, ModelOrigin origin)
            : base(name, ModelEntityClassifier.Class, TestModelEntityStereotypes.Interface, origin)
        {
        }

        public override int Priority => 1;
    }
}
