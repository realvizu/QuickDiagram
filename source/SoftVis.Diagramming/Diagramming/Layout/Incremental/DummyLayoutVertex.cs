using System;
using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// Dummy vertices are used to break long edges spanning more than two layers.
    /// They ensure that adjacent vertices in the layout graph are always on adjacent layers.
    /// </summary>
    internal class DummyLayoutVertex : LayoutVertexBase
    {
        private static int _nextId = 1;
        protected readonly int Id;

        public DummyLayoutVertex(int? id = null)
        {
            Id = id ?? SetNextUniqueId();
        }

        private static int SetNextUniqueId()
        {
            lock (typeof (DummyLayoutVertex))
            {
                var id = _nextId;
                _nextId++;
                return id;
            }
        }

        public static void ResetUniqueIdCounter() => _nextId = 1;

        public override bool IsDummy => true;
        public override string Name => $"Dummy#{Id}";

        public override int Priority
        {
            get { throw new InvalidOperationException("Dummy vertex has no priority."); }
        }

        public override Size2D Size => Size2D.Zero;

        public override bool Equals(object obj)
        {
            return obj is DummyLayoutVertex && ((DummyLayoutVertex) obj).Id == Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
