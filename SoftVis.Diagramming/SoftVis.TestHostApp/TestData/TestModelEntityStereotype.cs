using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.TestHostApp.TestData
{
    internal class TestModelEntityStereotype : ModelEntityStereotype
    {
        public static readonly ModelEntityStereotype Interface = new TestModelEntityStereotype("interface");

        private TestModelEntityStereotype(string name)
            :base(name)
        {
        }
    }
}
