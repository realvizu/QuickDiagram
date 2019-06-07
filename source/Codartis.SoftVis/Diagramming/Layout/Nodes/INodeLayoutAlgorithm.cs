using System.Collections.Generic;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Diagramming.Layout.Nodes
{
    public interface INodeLayoutAlgorithm
    {
        IDictionary<ModelNodeId, Point2D> Calculate(IEnumerable<IDiagramNode> nodes);
    }
}
