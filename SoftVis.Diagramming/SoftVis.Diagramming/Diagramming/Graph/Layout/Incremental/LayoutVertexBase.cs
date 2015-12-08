using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Diagramming.Graph.Layout.Incremental
{
    /// <summary>
    /// Abstract base class for vertices used in a layout graph. 
    /// </summary>
    internal abstract class LayoutVertexBase
    {
        public abstract bool IsDummy { get; }
        public abstract string Name { get; }
        public abstract int Priority { get; }

        public abstract Size2D Size { get; }
        public double Width => Size.Width;
        public double Height => Size.Height;

        public override string ToString() => Name;
    }
}
