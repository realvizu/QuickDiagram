using System.Collections.Generic;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Diagramming.Definition.Layout;
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
        [NotNull] private readonly LayoutUnifier _layoutUnifier;

        /// <summary>
        /// The padding around the rect of the child nodes in the child area of the container nodes.
        /// </summary>
        public double ChildrenAreaPadding { get; }

        public DiagramLayoutAlgorithm(
            [NotNull] ILayoutAlgorithmSelectionStrategy layoutAlgorithmSelectionStrategy,
            double childrenAreaPadding = 10)
        {
            _layoutAlgorithmSelectionStrategy = layoutAlgorithmSelectionStrategy;
            _layoutUnifier = new LayoutUnifier(childrenAreaPadding);
            ChildrenAreaPadding = childrenAreaPadding;
        }

        public GroupLayoutInfo Calculate(IDiagram diagram)
        {
            var layoutStructure = new DiagramLayoutStructure(diagram);
            var rootLayoutAlgorithm = _layoutAlgorithmSelectionStrategy.GetForRoot();
            var relativeLayoutInfo = LayoutRecursive(layoutStructure, layoutStructure.RootLayoutGroup, rootLayoutAlgorithm);
            var absoluteLayoutInfo = _layoutUnifier.CalculateAbsoluteLayout(relativeLayoutInfo);
            return absoluteLayoutInfo;
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

                layoutGroup.SetChildrenAreaSize(node.Id, childrenAreaLayoutInfo.Rect.Size);
                childLayoutByParentNodeId.Add(node.ShapeId, childrenAreaLayoutInfo);
            }

            var groupLayoutInfo = layoutAlgorithm.Calculate(layoutGroup);

            foreach (var boxLayoutInfo in groupLayoutInfo.Boxes)
            {
                if (childLayoutByParentNodeId.TryGetValue(boxLayoutInfo.BoxShape.ShapeId, out var childrenAreaLayoutInfo))
                    boxLayoutInfo.ChildrenArea = childrenAreaLayoutInfo;
            }

            return groupLayoutInfo;
        }
    }
}