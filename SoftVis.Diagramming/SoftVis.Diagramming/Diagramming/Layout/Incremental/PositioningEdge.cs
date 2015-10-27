using System.Diagnostics;
using QuickGraph;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// An edge in the positioning graph.
    /// </summary>
    [DebuggerDisplay("{ToString()}")]
    internal class PositioningEdge : Edge<PositioningVertexBase>
    {
        public PositioningEdge(PositioningVertexBase source, PositioningVertexBase target)
            : base(source, target)
        {
        }

        public override string ToString()
        {
            return $"{Source}->{Target}";
        }
    }
}
