namespace Codartis.SoftVis.Modeling
{
    /// <summary>
    /// Defines the built-in relationship specifications.
    /// </summary>
    public static class RelationshipSpecifications
    {
        public static readonly RelationshipSpecification BaseTypes =
            new RelationshipSpecification(ModelRelationshipDirection.Outgoing, ModelRelationshipType.Generalization, null);

        public static readonly RelationshipSpecification Subtypes =
            new RelationshipSpecification(ModelRelationshipDirection.Incoming, ModelRelationshipType.Generalization, null);
    }
}
