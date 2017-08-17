using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    public class TestModelRelationshipStereotype : ModelRelationshipStereotype
    {
        public static readonly ModelRelationshipStereotype Inheritance = new TestModelRelationshipStereotype(nameof(Inheritance));
        public static readonly ModelRelationshipStereotype Implementation = new TestModelRelationshipStereotype(nameof(Implementation));

        private TestModelRelationshipStereotype(string name) : base(name)
        {
        }
    }
}
