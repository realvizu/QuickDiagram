using System.Collections.Generic;
using Codartis.SoftVis.UI.Extensibility;
using Codartis.SoftVis.VisualStudioIntegration.Diagramming;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;

namespace Codartis.SoftVis.VisualStudioIntegration.UI
{
    /// <summary>
    /// Extends the built-in diagram behaviour for the roslyn-based model elements.
    /// </summary>
    internal class CustomDiagramBehaviourProvider : DefaultDiagramBehaviourProvider
    {
        private static readonly RelatedEntityDescriptor ImplementedInterfacesDescriptor =
            new RelatedEntityDescriptor(
                RoslynRelatedEntitySpecifications.ImplementedInterface, CustomConnectorTypes.Implementation);

        private static readonly RelatedEntityDescriptor ImplementerTypesDescriptor =
            new RelatedEntityDescriptor(
                RoslynRelatedEntitySpecifications.ImplementerType, CustomConnectorTypes.Implementation);

        public override IEnumerable<RelatedEntityDescriptor> GetRelatedEntityButtonDescriptors()
        {
            yield return BaseTypesDescriptor;
            yield return SubtypesDescriptor;
            yield return ImplementedInterfacesDescriptor;
            yield return ImplementerTypesDescriptor;
        }
    }
}
