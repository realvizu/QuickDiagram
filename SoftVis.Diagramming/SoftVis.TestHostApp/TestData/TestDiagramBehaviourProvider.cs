using System.Collections.Generic;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.UI.Extensibility;
using Codartis.SoftVis.UI.Geometry;

namespace Codartis.SoftVis.TestHostApp.TestData
{
    internal class TestDiagramBehaviourProvider : DefaultDiagramBehaviourProvider
    {
        private static readonly RelatedEntityMiniButtonDescriptor ImplementedInterfacesDescriptor =
            new RelatedEntityMiniButtonDescriptor(
                TestRelationshipSpecifications.ImplementedInterfaces, TestConnectorTypes.Implementation,
                new RectRelativeLocation(RectAlignment.TopMiddle, new Point2D(MiniButtonRadius * 1.2, MiniButtonOverlapParentBy)));

        private static readonly RelatedEntityMiniButtonDescriptor ImplementerTypesDescriptor =
            new RelatedEntityMiniButtonDescriptor(
                TestRelationshipSpecifications.ImplementerTypes, TestConnectorTypes.Implementation,
                new RectRelativeLocation(RectAlignment.BottomMiddle, new Point2D(MiniButtonRadius * 1.2, -MiniButtonOverlapParentBy)));

        public override IEnumerable<RelatedEntityMiniButtonDescriptor> GetRelatedEntityMiniButtonDescriptors()
        {
            yield return BaseTypesDescriptor.WithRelativeLocationTranslate(new Point2D(-MiniButtonRadius * 1.2, MiniButtonOverlapParentBy));
            yield return SubtypesDescriptor.WithRelativeLocationTranslate(new Point2D(-MiniButtonRadius * 1.2, -MiniButtonOverlapParentBy));
            yield return ImplementedInterfacesDescriptor;
            yield return ImplementerTypesDescriptor;
        }
    }
}
