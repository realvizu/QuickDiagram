namespace Codartis.SoftVis.Modeling
{
    /// <summary>
    /// Specifies related entities, by defining the relationship type and the direction to navigate it.
    /// </summary>
    public struct RelatedEntitySpecification
    {
        public ModelRelationshipTypeSpecification TypeSpecification { get; }
        public EntityRelationDirection Direction { get; }

        public RelatedEntitySpecification(ModelRelationshipType type, ModelRelationshipStereotype stereotype,
            EntityRelationDirection direction)
            : this(new ModelRelationshipTypeSpecification(type, stereotype), direction)
        {
        }

        public RelatedEntitySpecification(ModelRelationshipTypeSpecification typeSpecification, 
            EntityRelationDirection direction)
        {
            TypeSpecification = typeSpecification;
            Direction = direction;
        }

        public ModelRelationshipType Type => TypeSpecification.Type;
        public ModelRelationshipStereotype Stereotype => TypeSpecification.Stereotype;
    }
}
