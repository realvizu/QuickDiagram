using System;
using System.Linq;
using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// Dummy vertices are used to break long edges spanning more than two layers.
    /// They ensure that adjacent vertices in the positioning graph are always on adjacent layers.
    /// </summary>
    internal class DummyPositioningVertex : PositioningVertexBase
    {
        private static int _nextId = 1;
        private readonly int _id;

        public DummyPositioningVertex(PositioningGraph graph, bool isFloating)
            :base(graph, isFloating)
        {
            lock (typeof (DummyPositioningVertex))
            {
                _id = _nextId;
                _nextId++;
            }
        }

        public override string Name => ToString();
        public override int Priority => OutEdges.FirstOrDefault()?.Target.Priority ?? 0;
        public override double Width => 0d;
        public override double Height => 0d;
        public override Size2D Size => Size2D.Empty;
        public override int NonDummyLayerIndex => OutEdges.FirstOrDefault()?.Target.LayerIndex ?? LayerIndex;

        public override int CompareTo(PositioningVertexBase other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            return (other is DummyPositioningVertex) ? 0 : 1;
        }

        public override string ToString()
        {
            return $"Dummy#{_id}";
        }
    }
}
