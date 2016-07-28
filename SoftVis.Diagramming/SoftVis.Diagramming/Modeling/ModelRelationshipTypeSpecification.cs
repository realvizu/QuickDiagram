namespace Codartis.SoftVis.Modeling
{
    /// <summary>
    /// Describes the type and stereotype of a model relationship.
    /// </summary>
    public struct ModelRelationshipTypeSpecification
    {
        public ModelRelationshipType Type { get; }
        public ModelRelationshipStereotype Stereotype { get; }

        public ModelRelationshipTypeSpecification(ModelRelationshipType type, ModelRelationshipStereotype stereotype)
        {
            Type = type;
            Stereotype = stereotype;
        }
    }
}
