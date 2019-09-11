using Codartis.SoftVis.Geometry;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Implementation.Layout.Sugiyama
{
    /// <summary>
    /// Abstract base class for vertices used in a layout graph. 
    /// </summary>
    internal abstract class LayoutVertexBase
    {
        public abstract bool IsDummy { get; }
        [NotNull] public abstract string Name { get; }
        public abstract int Priority { get; }

        public abstract Size2D Size { get; }
        public double Width => Size.Width;
        public double Height => Size.Height;

        public override string ToString() => Name;
    }
}