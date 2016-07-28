using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling
{
    /// <summary>
    /// Defines the model relationship stereotypes used in Roslyn based models.
    /// </summary>
    internal static class ModelRelationshipStereotypes
    {
        public static readonly ModelRelationshipStereotype Implementation = new ModelRelationshipStereotype("implements");
    }
}
