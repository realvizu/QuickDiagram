using System.Collections.Generic;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Rendering.Extensibility
{
    /// <summary>
    /// The default implementation of a diagram extension provider.
    /// </summary>
    public class DefaultDiagramBehaviourProvider : IDiagramBehaviourProvider
    {
        public const double MiniButtonRadius = 8d;
        public const double MiniButtonOverlapParentBy = 3d;

        protected static readonly RelatedEntityMiniButtonDescriptor BaseTypesDescriptor =
            new RelatedEntityMiniButtonDescriptor(RelationshipSpecifications.BaseTypes, ConnectorTypes.Generalization,
                new RectRelativeLocation(RectReferencePoint.TopCenter, new Point2D(0, MiniButtonOverlapParentBy)));

        protected static readonly RelatedEntityMiniButtonDescriptor SubtypesDescriptor =
            new RelatedEntityMiniButtonDescriptor(RelationshipSpecifications.Subtypes, ConnectorTypes.Generalization,
                new RectRelativeLocation(RectReferencePoint.BottomCenter, new Point2D(0, -MiniButtonOverlapParentBy)));
        
        public virtual IEnumerable<RelatedEntityMiniButtonDescriptor> GetRelatedEntityMiniButtonDescriptors()
        {
            yield return BaseTypesDescriptor;
            yield return SubtypesDescriptor;
        }
    }
}
