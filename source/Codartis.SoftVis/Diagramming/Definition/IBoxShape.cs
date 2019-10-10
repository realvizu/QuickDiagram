using Codartis.SoftVis.Geometry;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Definition
{
    /// <summary>
    /// A rectangular diagram shape that consists of a payload area and a children area.
    /// </summary>
    /// <remarks>
    /// In the case of a diagram node that payload area contains the name of the node
    /// and the children area contains the sub-diagram of the child nodes.
    /// </remarks>
    public interface IBoxShape : IDiagramShape
    {
        [NotNull] string Name { get; }

        Size2D PayloadAreaSize { get; }
        Size2D ChildrenAreaSize { get; }
    }
}