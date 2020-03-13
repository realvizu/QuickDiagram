using JetBrains.Annotations;

namespace Codartis.SoftVis.UI
{
    /// <summary>
    /// Abstraction for the UI of a diagram shape.
    /// </summary>
    public interface IDiagramShapeUi
    {
        /// <summary>
        /// This a string to be able to unify node and connector stereotypes.
        /// </summary>
        [NotNull] string StereotypeName { get; }
    }
}