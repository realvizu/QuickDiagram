using System.Collections.Generic;
using System.Collections.Immutable;
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

        public DiagramLayoutInfo Calculate(ILayoutGroup layoutGroup)
        {
            var nodeTopLeftPositions = CalculateNodePositions(layoutGroup).ToImmutableDictionary(i => i.Key, i => i.Value);
            var connectorRoutes = CalculateConnectorRoutes(layoutGroup, nodeTopLeftPositions).ToImmutableDictionary(i => i.Key, i => i.Value);
            return new DiagramLayoutInfo(nodeTopLeftPositions, connectorRoutes);
        }

        [NotNull]
        private IEnumerable<KeyValuePair<ModelNodeId, Point2D>> CalculateNodePositions([NotNull] ILayoutGroup layoutGroup)
        {
            var orderedNodes = layoutGroup.Nodes.OrderBy(i => i.ModelNode.Name).ToList();

            double yPos = 0;
            foreach (var node in orderedNodes)
            {
                yield return new KeyValuePair<ModelNodeId, Point2D>(node.Id, new Point2D(0, yPos));

                yPos += node.Size.Height + _gapBetweenNodes;
            }
        }

        [NotNull]
        private static IEnumerable<KeyValuePair<ModelRelationshipId, Route>> CalculateConnectorRoutes(
            [NotNull] ILayoutGroup layoutGroup,
            [NotNull] IImmutableDictionary<ModelNodeId, Point2D> nodeTopLeftPositions)
        {
            return layoutGroup.Connectors.Select(i => new KeyValuePair<ModelRelationshipId, Route>(i.Id, GetRoute(layoutGroup, nodeTopLeftPositions, i)));
        }

        private static Route GetRoute(
            [NotNull] ILayoutGroup layoutGroup,
            [NotNull] IImmutableDictionary<ModelNodeId, Point2D> nodeTopLeftPositions,
            [NotNull] IDiagramConnector connector)
        {
            return new Route(
                GetNodeCenter(layoutGroup, nodeTopLeftPositions, connector.Source),
                GetNodeCenter(layoutGroup, nodeTopLeftPositions, connector.Target));
        }

        private static Point2D GetNodeCenter(
            [NotNull] ILayoutGroup layoutGroup,
            [NotNull] IImmutableDictionary<ModelNodeId, Point2D> nodeTopLeftPositions,
            ModelNodeId nodeId)
        {
            var originalNode = layoutGroup.GetNode(nodeId);
            return nodeTopLeftPositions.TryGetValue(nodeId, out var newTopLeftPosition)
                ? FromTopLeftToCenter(newTopLeftPosition, originalNode.Size)
                : originalNode.Center;
        }

        private static Point2D FromTopLeftToCenter(Point2D topLeft, Size2D size) => new Rect2D(topLeft, size).Center;
    }
}