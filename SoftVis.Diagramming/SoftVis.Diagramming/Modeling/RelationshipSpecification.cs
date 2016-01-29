namespace Codartis.SoftVis.Modeling
{
    /// <summary>
    /// Describes all characteristics of a model relationship.
    /// </summary>
    public class RelationshipSpecification
    {
        public ModelRelationshipDirection Direction { get; }
        public RelationshipTypeSpecification TypeSpecification { get; }

        public RelationshipSpecification(ModelRelationshipDirection direction,
            ModelRelationshipType type, ModelRelationshipStereotype stereotype)
            : this(direction, new RelationshipTypeSpecification(type, stereotype))
        {
        }

        public RelationshipSpecification(ModelRelationshipDirection direction,
            RelationshipTypeSpecification typeSpecification)
        {
            Direction = direction;
            TypeSpecification = typeSpecification;
        }

        public ModelRelationshipType Type => TypeSpecification.Type;
        public ModelRelationshipStereotype Stereotype => TypeSpecification.Stereotype;
    }
}
