namespace Codartis.SoftVis.Modeling
{
    /// <summary>
    /// Defines the built-in entity relation types.
    /// </summary>
    public static class EntityRelationTypes
    {
        public static readonly EntityRelationType BaseType = new EntityRelationType("Base types",
            ModelRelationshipClassifier.Generalization, ModelRelationshipStereotype.None, EntityRelationDirection.Outgoing);

        public static readonly EntityRelationType Subtype = new EntityRelationType("Derived types",
            ModelRelationshipClassifier.Generalization, ModelRelationshipStereotype.None, EntityRelationDirection.Incoming);
    }
}
