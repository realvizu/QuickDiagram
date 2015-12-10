using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling
{
    /// <summary>
    /// Extends the built-in model relationship specifications.
    /// </summary>
    internal static class CustomRelationshipSpecifications
    {
        public static readonly RelationshipSpecification ImplementedInterfaces =
            new RelationshipSpecification(ModelRelationshipDirection.Outgoing, ModelRelationshipType.Generalization,
                RoslynBasedModelRelationshipStereotype.Implementation);

        public static readonly RelationshipSpecification ImplementerTypes =
            new RelationshipSpecification(ModelRelationshipDirection.Incoming, ModelRelationshipType.Generalization,
                RoslynBasedModelRelationshipStereotype.Implementation);
    }
}