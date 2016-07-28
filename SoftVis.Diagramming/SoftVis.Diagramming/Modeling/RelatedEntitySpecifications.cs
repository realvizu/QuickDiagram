namespace Codartis.SoftVis.Modeling
{
    /// <summary>
    /// Defines the built-in related entity specifications.
    /// </summary>
    public static class RelatedEntitySpecifications
    {
        public static readonly RelatedEntitySpecification BaseType = new RelatedEntitySpecification(
            ModelRelationshipType.Generalization, 
            ModelRelationshipStereotype.None, 
            EntityRelationDirection.Outgoing);

        public static readonly RelatedEntitySpecification Subtype = new RelatedEntitySpecification(
            ModelRelationshipType.Generalization, 
            ModelRelationshipStereotype.None, 
            EntityRelationDirection.Incoming);
    }
}
