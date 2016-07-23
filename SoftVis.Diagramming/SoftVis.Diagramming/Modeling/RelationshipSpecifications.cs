namespace Codartis.SoftVis.Modeling
{
    /// <summary>
    /// Defines the built-in relationship specifications.
    /// </summary>
    public static class RelationshipSpecifications
    {
        public static readonly RelationshipSpecification BaseType =
            new RelationshipSpecification(ModelRelationshipDirection.Outgoing, ModelRelationshipType.Generalization, null);

        public static readonly RelationshipSpecification Subtype =
            new RelationshipSpecification(ModelRelationshipDirection.Incoming, ModelRelationshipType.Generalization, null);
    }
}
