using System.Collections.Generic;
using Codartis.SoftVis.UI.Extensibility;

namespace Codartis.SoftVis.TestHostApp.TestData
{
    internal class TestDiagramBehaviourProvider : DefaultDiagramBehaviourProvider
    {
        private static readonly RelatedEntityDescriptor ImplementedInterfacesDescriptor =
            new RelatedEntityDescriptor(
                TestRelationshipSpecifications.ImplementedInterfaces, TestConnectorTypes.Implementation);

        private static readonly RelatedEntityDescriptor ImplementerTypesDescriptor =
            new RelatedEntityDescriptor(
                TestRelationshipSpecifications.ImplementerTypes, TestConnectorTypes.Implementation);

        public override IEnumerable<RelatedEntityDescriptor> GetRelatedEntityButtonDescriptors()
        {
            yield return BaseTypesDescriptor;
            yield return SubtypesDescriptor;
            yield return ImplementedInterfacesDescriptor;
            yield return ImplementerTypesDescriptor;
        }
    }
}
