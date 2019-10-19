using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Diagramming.Definition.Layout;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling.Definition;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Implementation.Layout.Vertical
{
    /// <summary>
    /// A node layout algorithm that puts the nodes in a vertical column, ordered by their name.
    /// </summary>
    public sealed class VerticalNodeLayoutAlgorithm : IGroupLayoutAlgorithm
    {
        private readonly double _gapBetweenNodes;

        public VerticalNodeLayoutAlgorithm(double gapBetweenNodes = 2)
        {
            _gapBetweenNodes = gapBetweenNodes;
        }

        public LayoutInfo Calculate(ILayoutGroup layoutGroup)
        {
            var nodeLayout = CalculateNodeRects(layoutGroup);
            var connectorLayout = CalculateConnectorRoutes(layoutGroup, nodeLayout);
            return new LayoutInfo(nodeLayout, connectorLayout);
        }

        [NotNull]
        private IDictionary<ModelNodeId, Rect2D> CalculateNodeRects([NotNull] ILayoutGroup layoutGroup)
        {
            var result = new Dictionary<ModelNodeId, Rect2D>();
            var orderedNodes = layoutGroup.Nodes.OrderBy(i => i.ModelNode.Name).ToList();

            double yPos = 0;
            foreach (var node in orderedNodes)
            {
                result.Add(node.Id, new Rect2D(new Point2D(0, yPos), node.Size));

                yPos += node.Size.Height + _gapBetweenNodes;
            }

            return result;
        }

        [NotNull]
        private static IDictionary<ModelRelationshipId, Route> CalculateConnectorRoutes(
            [NotNull] ILayoutGroup layoutGroup,
            [NotNull] IDictionary<ModelNodeId, Rect2D> nodeRects)
        {
            return layoutGroup.Connectors.ToDictionary(i => i.Id, i => GetRoute(layoutGroup, nodeRects, i));
        }

        private static Route GetRoute(
            [NotNull] ILayoutGroup layoutGroup,
            [NotNull] IDictionary<ModelNodeId, Rect2D> nodeRects,
            [NotNull] IDiagramConnector connector)
        {
            return new Route(
                GetNodeCenter(layoutGroup, nodeRects, connector.Source),
                GetNodeCenter(layoutGroup, nodeRects, connector.Target));
        }

        private static Point2D GetNodeCenter(
            [NotNull] ILayoutGroup layoutGroup,
            [NotNull] IDictionary<ModelNodeId, Rect2D> nodeRects,
            ModelNodeId nodeId)
        {
            var originalNode = layoutGroup.GetNode(nodeId);
            var newTopLeftPosition = nodeRects[nodeId].TopLeft;
            return FromTopLeftToCenter(newTopLeftPosition, originalNode.Size);
        }

        private static Point2D FromTopLeftToCenter(Point2D topLeft, Size2D size) => new Rect2D(topLeft, size).Center;
    }
}