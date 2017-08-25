﻿using Codartis.SoftVis.Diagramming;

namespace Codartis.SoftVis.TestHostApp.Diagramming
{
    public static class TestConnectorTypes
    {
        public static readonly ConnectorType Generalization = new ConnectorType(ArrowHeadType.Hollow, LineType.Solid);
        public static readonly ConnectorType Implementation = new ConnectorType(ArrowHeadType.Hollow, LineType.Dashed);
    }
}
