using Codartis.SoftVis.Diagramming;

namespace Codartis.SoftVis.VisualStudioIntegration.Diagramming
{
    /// <summary>
    /// Extensions to the built-in connector types.
    /// </summary>
    internal static class CustomConnectorTypes
    {
        public static readonly ConnectorType Implementation = new ConnectorType(ArrowHeadType.Hollow, LineType.Dashed);
    }
}
