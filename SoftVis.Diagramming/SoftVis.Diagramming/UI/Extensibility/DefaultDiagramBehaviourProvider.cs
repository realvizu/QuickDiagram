using System.Collections.Generic;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.UI.Extensibility
{
    /// <summary>
    /// The default implementation of a diagram extension provider.
    /// </summary>
    public class DefaultDiagramBehaviourProvider : IDiagramBehaviourProvider
    {
        protected static readonly RelatedEntityDescriptor BaseTypesDescriptor =
            new RelatedEntityDescriptor(RelatedEntitySpecifications.BaseType, ConnectorTypes.Generalization);

        protected static readonly RelatedEntityDescriptor SubtypesDescriptor =
            new RelatedEntityDescriptor(RelatedEntitySpecifications.Subtype, ConnectorTypes.Generalization);
        
        public virtual IEnumerable<RelatedEntityDescriptor> GetRelatedEntityButtonDescriptors()
        {
            yield return BaseTypesDescriptor;
            yield return SubtypesDescriptor;
        }
    }
}
