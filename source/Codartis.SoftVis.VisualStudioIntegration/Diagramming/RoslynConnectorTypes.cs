﻿using Codartis.SoftVis.Diagramming.Definition;

namespace Codartis.SoftVis.VisualStudioIntegration.Diagramming
{
    /// <summary>
    /// Extensions to the built-in connector types.
    /// </summary>
    internal static class RoslynConnectorTypes
    {
        public static readonly ConnectorType Generalization = new ConnectorType(ArrowHeadType.Hollow, LineType.Solid);
        public static readonly ConnectorType Implementation = new ConnectorType(ArrowHeadType.Hollow, LineType.Dashed);
        public static readonly ConnectorType Association = new ConnectorType(ArrowHeadType.Simple, LineType.Solid);
    }
}