using Codartis.SoftVis.Modeling.Definition;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling
{
    /// <summary>
    /// Model relationship stereotypes extended for Roslyn based models.
    /// </summary>
    public static class ModelRelationshipStereotypes
    {
        public static readonly ModelRelationshipStereotype Inheritance = new ModelRelationshipStereotype(nameof(Inheritance));
        public static readonly ModelRelationshipStereotype Implementation = new ModelRelationshipStereotype(nameof(Implementation));
    }
}
