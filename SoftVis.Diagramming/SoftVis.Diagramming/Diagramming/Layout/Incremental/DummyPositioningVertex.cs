using System.Linq;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// Dummy vertices are used to break long edges spanning more than two layers.
    /// They ensure that adjacent vertices in the positioning graph are always on adjacent layers.
    /// </summary>
    internal class DummyPositioningVertex : PositioningVertexBase
    {
        private static int _nextId = 1;
        protected int Id;

        public DummyPositioningVertex(PositioningGraph graph, bool isFloating)
            :base(graph, isFloating)
        {
            lock (typeof (DummyPositioningVertex))
            {
                Id = _nextId;
                _nextId++;
            }
        }

        public override string Name => ToString();
        public override string NameForComparison => InEdges.FirstOrDefault()?.Source.NameForComparison ?? string.Empty;
        public override int Priority => OutEdges.FirstOrDefault()?.Target.Priority ?? 0;
        public override double Width => 0d;
        public override double Height => 0d;
        public override int GetNonDummyLayerIndex() => OutEdges.FirstOrDefault()?.Target.GetLayerIndex() ?? GetLayerIndex();

        public override string ToString()
        {
            return $"Dummy#{Id}";
        }
    }
}
