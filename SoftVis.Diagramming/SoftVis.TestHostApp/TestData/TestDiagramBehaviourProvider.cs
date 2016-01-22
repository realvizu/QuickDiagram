using System.Collections.Generic;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.UI.Extensibility;
using Codartis.SoftVis.UI.Geometry;

namespace Codartis.SoftVis.TestHostApp.TestData
{
    internal class TestDiagramBehaviourProvider : DefaultDiagramBehaviourProvider
    {
        private static readonly RelatedEntityButtonDescriptor ImplementedInterfacesDescriptor =
            new RelatedEntityButtonDescriptor(
                TestRelationshipSpecifications.ImplementedInterfaces, TestConnectorTypes.Implementation,
                new RectRelativeLocation(RectAlignment.TopMiddle, new Point2D(ButtonRadius * 1.2, ButtonOverlapParentBy)));

        private static readonly RelatedEntityButtonDescriptor ImplementerTypesDescriptor =
            new RelatedEntityButtonDescriptor(
                TestRelationshipSpecifications.ImplementerTypes, TestConnectorTypes.Implementation,
                new RectRelativeLocation(RectAlignment.BottomMiddle, new Point2D(ButtonRadius * 1.2, -ButtonOverlapParentBy)));

        public override IEnumerable<RelatedEntityButtonDescriptor> GetRelatedEntityButtonDescriptors()
        {
            yield return BaseTypesDescriptor.WithRelativeLocationTranslate(new Point2D(-ButtonRadius * 1.2, ButtonOverlapParentBy));
            yield return SubtypesDescriptor.WithRelativeLocationTranslate(new Point2D(-ButtonRadius * 1.2, -ButtonOverlapParentBy));
            yield return ImplementedInterfacesDescriptor;
            yield return ImplementerTypesDescriptor;
        }
    }
}
