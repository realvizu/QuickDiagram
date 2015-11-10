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

        public DummyLayoutVertex(bool isFloating, int? id = null)
            :base(isFloating)
        {
            if (id.HasValue)
            {
                Id = id.Value;
            }
            else
            {
                lock (typeof (DummyLayoutVertex))
                {
                    Id = _nextId;
                    _nextId++;
                }
            }
        }

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
