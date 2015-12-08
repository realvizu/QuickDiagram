using System.Collections.Generic;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Rendering.Extensibility;

namespace Codartis.SoftVis.TestHostApp.TestData
{
    internal class TestDiagramBehaviourProvider : DefaultDiagramBehaviourProvider
    {
        private static readonly RelatedEntityMiniButtonDescriptor ImplementedInterfacesDescriptor =
            new RelatedEntityMiniButtonDescriptor(
                TestRelationshipSpecifications.ImplementedInterfaces, TestConnectorTypes.Implementation,
                new RectRelativeLocation(RectReferencePoint.TopCenter, new Point2D(MiniButtonRadius * 1.2, MiniButtonOverlapParentBy)));

        private static readonly RelatedEntityMiniButtonDescriptor ImplementerTypesDescriptor =
            new RelatedEntityMiniButtonDescriptor(
                TestRelationshipSpecifications.ImplementerTypes, TestConnectorTypes.Implementation,
                new RectRelativeLocation(RectReferencePoint.BottomCenter, new Point2D(MiniButtonRadius * 1.2, -MiniButtonOverlapParentBy)));

        public override IEnumerable<RelatedEntityMiniButtonDescriptor> GetRelatedEntityMiniButtonDescriptors()
        {
            yield return BaseTypesDescriptor.WithRelativeLocationTranslate(new Point2D(-MiniButtonRadius * 1.2, MiniButtonOverlapParentBy));
            yield return SubtypesDescriptor.WithRelativeLocationTranslate(new Point2D(-MiniButtonRadius * 1.2, -MiniButtonOverlapParentBy));
            yield return ImplementedInterfacesDescriptor;
            yield return ImplementerTypesDescriptor;
        }
    }
}
