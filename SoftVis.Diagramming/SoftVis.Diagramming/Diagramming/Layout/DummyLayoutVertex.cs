using System;
using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Diagramming.Layout
{
    /// <summary>
    /// Dummy vertices are used to break long edges spanning more than two layers.
    /// They ensure that adjacent vertices in the graph are always on adjacent layers
    /// </summary>
    internal class DummyLayoutVertex : LayoutVertexBase
    {
        private static int _nextId = 1;
        private readonly int _id;

        public DummyLayoutVertex(LayoutGraph graph, bool isFloating)
            :base(graph, isFloating)
        {
            lock (typeof (DummyLayoutVertex))
            {
                _id = _nextId;
                _nextId++;
            }
        }

        public override int Priority => 0;
        public override double Width => 0d;
        public override double Height => 0d;
        public override Size2D Size => Size2D.Empty;

        public override int CompareTo(LayoutVertexBase other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            return (other is DummyLayoutVertex) ? 0 : 1;
        }

        public override string ToString()
        {
            return $"(dummy #{_id})";
        }
    }
}
