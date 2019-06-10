using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Diagramming.Layout.Nodes
{
    /// <summary>
    /// Calculates diagram node layout.
    /// </summary>
    public interface INodeLayoutAlgorithm
    {
        /// <summary>
        /// Calculates the position of a collection of diagram nodes.
        /// </summary>
        /// <returns>A dictionary where key = node ID, value = top left corner coordinates.</returns>
        [Pure]
        IDictionary<ModelNodeId, Point2D> Calculate(IEnumerable<IDiagramNode> nodes, IEnumerable<IDiagramConnector> connectors);
    }
}
