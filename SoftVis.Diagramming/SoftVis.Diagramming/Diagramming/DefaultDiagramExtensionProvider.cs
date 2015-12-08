using System.Collections.Generic;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Diagramming
{
    /// <summary>
    /// The default implementation of a diagram extension provider.
    /// </summary>
    public class DefaultDiagramExtensionProvider : IDiagramExtensionProvider
    {
        public const double MiniButtonRadius = 8d;
        public const double MiniButtonOverlapParentBy = 3d;

        private static readonly RelatedEntityMiniButtonDescriptor BaseTypes =
            new RelatedEntityMiniButtonDescriptor(ModelRelationshipDirection.Outgoing, ModelRelationshipType.Generalization, null,
                new RectRelativeLocation(RectReferencePoint.TopCenter, new Point2D(0, MiniButtonOverlapParentBy)),
                BuiltInConnectorStyles.Generalization);

        private static readonly RelatedEntityMiniButtonDescriptor Subtypes =
            new RelatedEntityMiniButtonDescriptor(ModelRelationshipDirection.Incoming, ModelRelationshipType.Generalization, null,
                new RectRelativeLocation(RectReferencePoint.BottomCenter, new Point2D(0, -MiniButtonOverlapParentBy)),
                BuiltInConnectorStyles.Generalization);
        
        public virtual IEnumerable<RelatedEntityMiniButtonDescriptor> GetRelatedEntityMiniButtonDescriptors()
        {
            yield return BaseTypes;
            yield return Subtypes;
        }

        public virtual ConnectorStyle GetConnectorStyle(IModelRelationship modelRelationship)
        {
            return BuiltInConnectorStyles.Generalization;
        }
    }
}
