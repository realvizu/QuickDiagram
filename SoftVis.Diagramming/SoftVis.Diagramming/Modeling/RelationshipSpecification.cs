namespace Codartis.SoftVis.Modeling
{
    /// <summary>
    /// Describes all characteristics of a model relationship.
    /// </summary>
    public class RelationshipSpecification
    {
        public ModelRelationshipDirection Direction { get; }
        public ModelRelationshipType Type { get; }
        public ModelRelationshipStereotype Stereotype { get; }

        public RelationshipSpecification(ModelRelationshipDirection direction, 
            ModelRelationshipType type, ModelRelationshipStereotype stereotype)
        {
            Direction = direction;
            Type = type;
            Stereotype = stereotype;
        }
    }
}
