using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.UI.Extensibility
{
    /// <summary>
    /// Describes the properties needed to show related entities.
    /// </summary>
    public class RelatedEntityDescriptor
    {
        public RelatedEntitySpecification RelatedEntitySpecification { get; }
        public ConnectorType ConnectorType { get; }

        public RelatedEntityDescriptor(RelatedEntitySpecification relatedEntitySpecification, ConnectorType connectorType)
        {
            RelatedEntitySpecification = relatedEntitySpecification;
            ConnectorType = connectorType;
        }
    }
}
