using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Diagramming.Layout.Nodes.Vertical
{
    /// <summary>
    /// A node layout algorithm that puts the nodes in a vertical column, ordered by their name.
    /// </summary>
    public sealed class VerticalNodeLayoutAlgorithm : INodeLayoutAlgorithm
    {
        private const double Margin = 2;

        public IDictionary<ModelNodeId, Point2D> Calculate(IEnumerable<IDiagramNode> nodes)
        {
            var result = new Dictionary<ModelNodeId, Point2D>();

            var orderedNodes = nodes.OrderBy(i => i.Name);

            double yPos = 0;
            foreach (var node in orderedNodes)
            {
                var position = new Point2D(0, yPos);
                result.Add(node.Id, position);

                yPos += node.Size.Height + Margin;
            }

            return result;
        }
    }
}
