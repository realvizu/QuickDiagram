using System.Diagnostics;

namespace Codartis.SoftVis.Diagramming.Graph.Layout.VertexPlacement.EfficientSugiyama
{
    [DebuggerDisplay("First = {First}, Second = {Second}")]
    internal class Pair
    {
        public int First;
        public int Second;
        public int Weight = 1;
    }
}