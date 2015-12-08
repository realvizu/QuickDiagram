using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.TestHostApp.TestData
{
    internal class TestConnectorTypeResolver : DefaultConnectorTypeResolver
    {
        public override ConnectorType GetConnectorType(IModelRelationship modelRelationship)
        {
            return modelRelationship.Stereotype == TestModelRelationshipStereotype.Implementation
                ? TestConnectorTypes.Implementation
                : base.GetConnectorType(modelRelationship);
        }
    }
}
