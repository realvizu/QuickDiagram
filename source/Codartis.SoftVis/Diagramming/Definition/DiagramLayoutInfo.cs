using System.Collections.Generic;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Definition
{
    public struct DiagramLayoutInfo
    {
        [NotNull] public IEnumerable<NodeLayoutInfo> RootNodes { get; }
        [NotNull] public IEnumerable<ConnectorLayoutInfo> RootConnectors { get; }

        public DiagramLayoutInfo(
            [NotNull] IEnumerable<NodeLayoutInfo> rootNodes,
            [NotNull] IEnumerable<ConnectorLayoutInfo> rootConnectors)
        {
            RootNodes = rootNodes;
            RootConnectors = rootConnectors;
        }
    }
}