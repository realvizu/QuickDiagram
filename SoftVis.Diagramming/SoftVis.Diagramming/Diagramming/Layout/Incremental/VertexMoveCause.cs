using System.Diagnostics;
using QuickGraph;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    [DebuggerDisplay("{Reason}")]
    internal class VertexMoveCause : Edge<VertexMove>
    {
        public string Reason { get; }

        public VertexMoveCause(VertexMove source, VertexMove target, string reason)
            : base(source, target)
        {
            Reason = reason;
        }
    }
}
