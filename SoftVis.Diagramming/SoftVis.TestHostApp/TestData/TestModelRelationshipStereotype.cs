using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.TestHostApp.TestData
{
    internal class TestModelRelationshipStereotype : ModelRelationshipStereotype
    {
        public static readonly ModelRelationshipStereotype Implementation = new TestModelRelationshipStereotype("implements");

        private TestModelRelationshipStereotype(string name)
            :base(name)
        {
        }
    }
}
