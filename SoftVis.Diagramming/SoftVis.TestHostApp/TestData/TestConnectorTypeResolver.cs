using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.TestHostApp.TestData
{
    internal class TestConnectorTypeResolver : IConnectorTypeResolver
    {
        public ConnectorType GetConnectorType(IModelRelationship modelRelationship)
        {
            return modelRelationship.Stereotype == TestModelRelationshipStereotypes.Implementation
                ? TestConnectorTypes.Implementation
                : ConnectorTypes.Generalization;
        }
    }
}
