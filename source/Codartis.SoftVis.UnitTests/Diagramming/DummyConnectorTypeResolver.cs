using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Modeling.Definition;

namespace Codartis.SoftVis.UnitTests.Diagramming
{
    /// <summary>
    /// A dummy resolver that always returns Dependency connector type.
    /// </summary>
    public sealed class DummyConnectorTypeResolver : IConnectorTypeResolver
    {
        public ConnectorType GetConnectorType(ModelRelationshipStereotype stereotype) => ConnectorTypes.Dependency;
    }
}