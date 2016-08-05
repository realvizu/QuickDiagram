using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling
{
    /// <summary>
    /// Extends the built-in model relationship specifications.
    /// </summary>
    internal static class RoslynEntityRelationTypes
    {
        public static readonly EntityRelationType ImplementedInterface = new EntityRelationType(
            ModelRelationshipClassifier.Generalization,
            ModelRelationshipStereotypes.Implementation,
            EntityRelationDirection.Outgoing);

        public static readonly EntityRelationType ImplementerType = new EntityRelationType(
            ModelRelationshipClassifier.Generalization,
            ModelRelationshipStereotypes.Implementation,
            EntityRelationDirection.Incoming);
    }
}