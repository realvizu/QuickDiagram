namespace Codartis.SoftVis.Modeling
{
    /// <summary>
    /// Describes the type of a model relationship.
    /// </summary>
    public class RelationshipTypeSpecification
    {
        public ModelRelationshipType Type { get; }
        public ModelRelationshipStereotype Stereotype { get; }

        public RelationshipTypeSpecification(ModelRelationshipType type, ModelRelationshipStereotype stereotype)
        {
            Type = type;
            Stereotype = stereotype;
        }
    }
}
