using Codartis.SoftVis.Modeling2;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling
{
    /// <summary>
    /// Model relationship stereotypes extended for Roslyn based models.
    /// </summary>
    public class RoslynModelRelationshipStereotype : ModelRelationshipStereotype
    {
        public static readonly ModelRelationshipStereotype Inheritance = new RoslynModelRelationshipStereotype(nameof(Inheritance));
        public static readonly ModelRelationshipStereotype Implementation = new RoslynModelRelationshipStereotype(nameof(Implementation));

        private RoslynModelRelationshipStereotype(string name) : base(name)
        {
        }
    }
}
