using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.UI.Geometry;

namespace Codartis.SoftVis.UI.Extensibility
{
    /// <summary>
    /// Describes the properties of a button for showing related entities.
    /// </summary>
    public class RelatedEntityButtonDescriptor
    {
        public RelationshipSpecification RelationshipSpecification { get; }
        public ConnectorType ConnectorType { get; }
        public RectRelativeLocation ButtonLocation { get; }

        public RelatedEntityButtonDescriptor(RelationshipSpecification relationshipSpecification, 
            ConnectorType connectorType, RectRelativeLocation buttonLocation)
        {
            RelationshipSpecification = relationshipSpecification;
            ConnectorType = connectorType;
            ButtonLocation = buttonLocation;
        }

        public RelatedEntityButtonDescriptor WithRelativeLocation(RectRelativeLocation relativeLocation)
        {
            return new RelatedEntityButtonDescriptor(RelationshipSpecification, ConnectorType, relativeLocation);
        }

        public RelatedEntityButtonDescriptor WithRelativeLocationTranslate(Point2D translate)
        {
            return new RelatedEntityButtonDescriptor(RelationshipSpecification, ConnectorType, ButtonLocation.WithTranslate(translate));
        }
    }
}
