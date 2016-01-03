using System.Collections.Generic;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Rendering.Geometry;

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
                new RectRelativeLocation(RectAlignment.TopMiddle, new Point2D(0, MiniButtonOverlapParentBy)));

        protected static readonly RelatedEntityMiniButtonDescriptor SubtypesDescriptor =
            new RelatedEntityMiniButtonDescriptor(RelationshipSpecifications.Subtypes, ConnectorTypes.Generalization,
                new RectRelativeLocation(RectAlignment.BottomMiddle, new Point2D(0, -MiniButtonOverlapParentBy)));
        
        public virtual IEnumerable<RelatedEntityMiniButtonDescriptor> GetRelatedEntityMiniButtonDescriptors()
        {
            yield return BaseTypesDescriptor;
            yield return SubtypesDescriptor;
        }
    }
}
