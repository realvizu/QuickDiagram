using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Diagramming
{
    /// <summary>
    /// Describes the properties of a button for showing related entities.
    /// </summary>
    public class RelatedEntityMiniButtonDescriptor
    {
        public ModelRelationshipDirection RelationshipDirection { get; }
        public ModelRelationshipType RelationshipType { get; }
        public ModelRelationshipStereotype RelationshipStereotype { get; }
        public RectRelativeLocation Location { get; }
        public ConnectorStyle ConnectorStyle { get; }

        public RelatedEntityMiniButtonDescriptor(
            ModelRelationshipDirection relationshipDirection,
            ModelRelationshipType relationshipType, 
            ModelRelationshipStereotype relationshipStereotype,
            RectRelativeLocation location,
            ConnectorStyle connectorStyle
            )
        {
            RelationshipDirection = relationshipDirection;
            RelationshipType = relationshipType;
            RelationshipStereotype = relationshipStereotype;
            Location = location;
            ConnectorStyle = connectorStyle;
        }
    }
}
