using System;
using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Diagramming.Implementation.Layout.Sugiyama
{
    /// <summary>
    /// Dummy vertices are used to break long edges spanning more than two layers.
    /// They ensure that adjacent vertices in the layout graph are always on adjacent layers.
    /// </summary>
    internal class DummyLayoutVertex : LayoutVertexBase
    {
        protected readonly int Id;

        public DummyLayoutVertex(int id)
        {
            Id = id;
        }

        public override bool IsDummy => true;
        public override string Name => $"Dummy#{Id}";
        public override int Priority => throw new InvalidOperationException("Dummy vertex has no priority.");
        public override Size2D Size => Size2D.Zero;

        public override bool Equals(object obj)
        {
            return obj is DummyLayoutVertex && ((DummyLayoutVertex)obj).Id == Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
