using Codartis.SoftVis.Modeling.Definition;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    public static class ModelRelationshipStereotypes 
    {
        public static readonly ModelRelationshipStereotype Association = new ModelRelationshipStereotype(nameof(Association));
        public static readonly ModelRelationshipStereotype Implementation = new ModelRelationshipStereotype(nameof(Implementation));
        public static readonly ModelRelationshipStereotype Inheritance = new ModelRelationshipStereotype(nameof(Inheritance));
    }
}
