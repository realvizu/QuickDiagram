using System.Collections.Immutable;
using Codartis.SoftVis.Geometry;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming
{
    /// <summary>
    /// A part of a diagram that is laid out together.
    /// Consists of diagram nodes and connectors that form a directed graph.
    /// </summary>
    public interface ILayoutGroup
    {
        [NotNull] IImmutableSet<IDiagramNode> Nodes { get; }
        [NotNull] IImmutableSet<IDiagramConnector> Connectors { get; }

        bool IsEmpty { get; }

        /// <summary>
        /// The absolute position and size of the layout group.
        /// </summary>
        Rect2D Rect { get; }
    }
}