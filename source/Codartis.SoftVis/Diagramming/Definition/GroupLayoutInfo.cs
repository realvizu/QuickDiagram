using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Geometry;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Definition
{
    public sealed class GroupLayoutInfo : ILayoutInfo
    {
        [NotNull] public IEnumerable<NodeLayoutInfo> Nodes { get; }
        [NotNull] public IEnumerable<ConnectorLayoutInfo> Connectors { get; }
        public Rect2D Rect { get; }

        public GroupLayoutInfo(
            IEnumerable<NodeLayoutInfo> nodes = null,
            IEnumerable<ConnectorLayoutInfo> connectors = null)
        {
            Nodes = nodes ?? Enumerable.Empty<NodeLayoutInfo>();
            Connectors = connectors ?? Enumerable.Empty<ConnectorLayoutInfo>();
            Rect = Nodes.Select(i => i.Rect).Concat(Connectors.Select(i => i.Rect)).Union();
        }
    }
}