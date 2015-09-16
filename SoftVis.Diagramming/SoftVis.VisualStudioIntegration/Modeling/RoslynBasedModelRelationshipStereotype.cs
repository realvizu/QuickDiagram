using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling
{
    /// <summary>
    /// Defines the model relationship stereotypes used in Roslyn based models.
    /// </summary>
    internal class RoslynBasedModelRelationshipStereotype : ModelRelationshipStereotype
    {
        public static readonly ModelRelationshipStereotype Implementation = new RoslynBasedModelRelationshipStereotype("implements");

        private RoslynBasedModelRelationshipStereotype(string name)
            :base(name)
        {
        }
    }
}
