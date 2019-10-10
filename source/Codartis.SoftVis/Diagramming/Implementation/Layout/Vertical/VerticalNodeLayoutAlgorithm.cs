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

        public GroupLayoutInfo Calculate(ILayoutGroup layoutGroup)
        {
            var nodeLayout = CalculateNodePositions(layoutGroup).ToList();
            var connectorLayout = CalculateConnectorRoutes(layoutGroup, nodeLayout).ToList();
            return new GroupLayoutInfo(nodeLayout, connectorLayout);
        }

        [NotNull]
        private IEnumerable<BoxLayoutInfo> CalculateNodePositions([NotNull] ILayoutGroup layoutGroup)
        {
            var orderedNodes = layoutGroup.Nodes.OrderBy(i => i.ModelNode.Name).ToList();

            double yPos = 0;
            foreach (var node in orderedNodes)
            {
                yield return new BoxLayoutInfo(node, new Point2D(0, yPos));

                yPos += node.Size.Height + _gapBetweenNodes;
            }
        }

        [NotNull]
        private static IEnumerable<LineLayoutInfo> CalculateConnectorRoutes(
            [NotNull] ILayoutGroup layoutGroup,
            [NotNull] IList<BoxLayoutInfo> nodeTopLeftPositions)
        {
            return layoutGroup.Connectors.Select(i => new LineLayoutInfo(i, GetRoute(layoutGroup, nodeTopLeftPositions, i)));
        }

        private static Route GetRoute(
            [NotNull] ILayoutGroup layoutGroup,
            [NotNull] IList<BoxLayoutInfo> nodeLayout,
            [NotNull] IDiagramConnector connector)
        {
            return new Route(
                GetNodeCenter(layoutGroup, nodeLayout, connector.Source),
                GetNodeCenter(layoutGroup, nodeLayout, connector.Target));
        }

        private static Point2D GetNodeCenter(
            [NotNull] ILayoutGroup layoutGroup,
            [NotNull] IList<BoxLayoutInfo> nodeLayout,
            ModelNodeId nodeId)
        {
            var originalNode = layoutGroup.GetNode(nodeId);
            var newTopLeftPosition = nodeLayout.Single(i => i.BoxShape.ShapeId == nodeId.ToString()).Rect.TopLeft;
            return FromTopLeftToCenter(newTopLeftPosition, originalNode.Size);
        }

        private static Point2D FromTopLeftToCenter(Point2D topLeft, Size2D size) => new Rect2D(topLeft, size).Center;
    }
}