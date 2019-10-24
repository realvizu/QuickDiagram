using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Diagramming.Definition.Layout;
using Codartis.SoftVis.Geometry;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Implementation.Layout
{
    /// <summary>
    /// Traverses the diagram's node hierarchy, calculates layout for each node that has children
    /// and assembles the results into a <see cref="GroupLayoutInfo"/>.
    /// </summary>
    public sealed class DiagramLayoutAlgorithm : IDiagramLayoutAlgorithm
    {
        [NotNull] private readonly ILayoutAlgorithmSelectionStrategy _layoutAlgorithmSelectionStrategy;
        [NotNull] private readonly IConnectorRoutingAlgorithm _crossLayoutGroupConnectorRoutingAlgorithm;
        [NotNull] private readonly LayoutUnifier _layoutUnifier;

        /// <summary>
        /// The padding around the rect of the child nodes in the child area of the container nodes.
        /// </summary>
        public double ChildrenAreaPadding { get; }

        public DiagramLayoutAlgorithm(
            [NotNull] ILayoutAlgorithmSelectionStrategy layoutAlgorithmSelectionStrategy,
            [NotNull] IConnectorRoutingAlgorithm crossLayoutGroupConnectorRoutingAlgorithm,
            double childrenAreaPadding = 2)
        {
            _layoutAlgorithmSelectionStrategy = layoutAlgorithmSelectionStrategy;
            _crossLayoutGroupConnectorRoutingAlgorithm = crossLayoutGroupConnectorRoutingAlgorithm;
            _layoutUnifier = new LayoutUnifier(childrenAreaPadding);
            ChildrenAreaPadding = childrenAreaPadding;
        }

        public GroupLayoutInfo Calculate(IDiagram diagram)
        {
            var layoutStructure = new DiagramLayoutStructure(diagram);

            var rootLayoutAlgorithm = _layoutAlgorithmSelectionStrategy.GetForRoot();

            var relativeLayout = LayoutRecursive(layoutStructure, layoutStructure.RootLayoutGroup, rootLayoutAlgorithm);

            var absoluteLayout = _layoutUnifier.CalculateAbsoluteLayout(relativeLayout);

            var crossGroupLines = CreateDirectRoutes(layoutStructure.CrossLayoutGroupConnectors, diagram, absoluteLayout);

            return absoluteLayout.AddLineLayoutInfo(crossGroupLines);
        }

        [NotNull]
        private IEnumerable<LineLayoutInfo> CreateDirectRoutes(
            [NotNull] [ItemNotNull] IEnumerable<IDiagramConnector> connectors,
            [NotNull] IDiagram diagram,
            [NotNull] GroupLayoutInfo groupLayoutInfo)
        {
            var nodeRects = diagram.Nodes.ToDictionary(i => i.Id, i => groupLayoutInfo.GetBoxById(i.ShapeId).Rect);
            var connectorRoutes = _crossLayoutGroupConnectorRoutingAlgorithm.Calculate(connectors, nodeRects);
            return connectorRoutes.Select(i => new LineLayoutInfo(i.Key.ToShapeId(), i.Value));
        }

        [NotNull]
        private GroupLayoutInfo LayoutRecursive(
            [NotNull] DiagramLayoutStructure layoutStructure,
            [NotNull] ILayoutGroup layoutGroup,
            [NotNull] IGroupLayoutAlgorithm layoutAlgorithm)
        {
            var childLayoutByParentNodeId = new Dictionary<string, GroupLayoutInfo>();

            foreach (var node in layoutGroup.Nodes)
            {
                var maybeLayoutGroup = layoutStructure.TryGetLayoutGroupByNodeId(node.Id);
                if (!maybeLayoutGroup.HasValue || maybeLayoutGroup.Value.IsEmpty)
                    continue;

                var nodeLayoutAlgorithm = _layoutAlgorithmSelectionStrategy.GetForNode(node);
                var childrenAreaLayoutInfo = LayoutRecursive(layoutStructure, maybeLayoutGroup.Value, nodeLayoutAlgorithm);

                layoutGroup.SetChildrenAreaSize(node.Id, childrenAreaLayoutInfo.Rect.Size.WithMargin(ChildrenAreaPadding));
                childLayoutByParentNodeId.Add(node.ShapeId, childrenAreaLayoutInfo);
            }

            var layoutInfo = layoutAlgorithm.Calculate(layoutGroup);

            var boxes = layoutGroup.Nodes.Select(
                i =>
                {
                    childLayoutByParentNodeId.TryGetValue(i.ShapeId, out var childrenAreaLayoutInfo);

                    return new BoxLayoutInfo(
                        i.ShapeId,
                        layoutInfo.VertexRects[i.Id].TopLeft,
                        payloadAreaSize: i.PayloadAreaSize,
                        childrenAreaLayoutInfo?.Rect.Size.WithMargin(ChildrenAreaPadding) ?? Size2D.Zero,
                        childrenAreaLayoutInfo);
                }
            );

            var lines = layoutGroup.Connectors.Select(i => new LineLayoutInfo(i.ShapeId, layoutInfo.EdgeRoutes[i.Id]));

            return new GroupLayoutInfo(boxes, lines);
        }
    }
}