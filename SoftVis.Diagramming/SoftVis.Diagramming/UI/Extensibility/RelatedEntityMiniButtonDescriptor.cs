using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.UI.Geometry;

namespace Codartis.SoftVis.UI.Extensibility
{
    /// <summary>
    /// Describes the properties of a button for showing related entities.
    /// </summary>
    public class RelatedEntityMiniButtonDescriptor
    {
        public RelationshipSpecification RelationshipSpecification { get; }
        public ConnectorType ConnectorType { get; }
        public RectRelativeLocation MiniButtonLocation { get; }

        public RelatedEntityMiniButtonDescriptor(RelationshipSpecification relationshipSpecification, 
            ConnectorType connectorType, RectRelativeLocation miniButtonLocation)
        {
            RelationshipSpecification = relationshipSpecification;
            ConnectorType = connectorType;
            MiniButtonLocation = miniButtonLocation;
        }

        public RelatedEntityMiniButtonDescriptor WithRelativeLocation(RectRelativeLocation relativeLocation)
        {
            return new RelatedEntityMiniButtonDescriptor(RelationshipSpecification, ConnectorType, relativeLocation);
        }

        public RelatedEntityMiniButtonDescriptor WithRelativeLocationTranslate(Point2D translate)
        {
            return new RelatedEntityMiniButtonDescriptor(RelationshipSpecification, ConnectorType, MiniButtonLocation.WithTranslate(translate));
        }
    }
}
