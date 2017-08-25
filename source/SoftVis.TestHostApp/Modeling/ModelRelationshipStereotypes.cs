using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    public static class ModelRelationshipStereotypes 
    {
        public static readonly ModelRelationshipStereotype Inheritance = new ModelRelationshipStereotype(nameof(Inheritance));
        public static readonly ModelRelationshipStereotype Implementation = new ModelRelationshipStereotype(nameof(Implementation));
    }
}
