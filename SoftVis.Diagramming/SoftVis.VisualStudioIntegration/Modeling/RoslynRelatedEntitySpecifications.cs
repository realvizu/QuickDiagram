using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling
{
    /// <summary>
    /// Extends the built-in model relationship specifications.
    /// </summary>
    internal static class RoslynRelatedEntitySpecifications
    {
        public static readonly RelatedEntitySpecification ImplementedInterface = new RelatedEntitySpecification(
            ModelRelationshipType.Generalization,
            ModelRelationshipStereotypes.Implementation,
            EntityRelationDirection.Outgoing);

        public static readonly RelatedEntitySpecification ImplementerType = new RelatedEntitySpecification(
            ModelRelationshipType.Generalization,
            ModelRelationshipStereotypes.Implementation,
            EntityRelationDirection.Incoming);
    }
}