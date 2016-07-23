using System.Collections.Generic;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.UI.Geometry;

namespace Codartis.SoftVis.UI.Extensibility
{
    /// <summary>
    /// The default implementation of a diagram extension provider.
    /// </summary>
    public class DefaultDiagramBehaviourProvider : IDiagramBehaviourProvider
    {
        public const double ButtonRadius = 8d;
        public const double ButtonOverlapParentBy = 3d;

        protected static readonly RelatedEntityButtonDescriptor BaseTypesDescriptor =
            new RelatedEntityButtonDescriptor(RelationshipSpecifications.BaseType, ConnectorTypes.Generalization,
                new RectRelativeLocation(RectAlignment.TopMiddle, new Point2D(0, ButtonOverlapParentBy)));

        protected static readonly RelatedEntityButtonDescriptor SubtypesDescriptor =
            new RelatedEntityButtonDescriptor(RelationshipSpecifications.Subtype, ConnectorTypes.Generalization,
                new RectRelativeLocation(RectAlignment.BottomMiddle, new Point2D(0, -ButtonOverlapParentBy)));
        
        public virtual IEnumerable<RelatedEntityButtonDescriptor> GetRelatedEntityButtonDescriptors()
        {
            yield return BaseTypesDescriptor;
            yield return SubtypesDescriptor;
        }
    }
}
