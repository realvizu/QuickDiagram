using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.TestHostApp.TestData
{
    internal class TestClass : TestModelEntity
    {
        public TestClass(string name, ModelOrigin origin)
            : base(name, ModelEntityClassifier.Class, ModelEntityStereotype.None, origin)
        {
        }

        public override int Priority => 2;
    }
}
