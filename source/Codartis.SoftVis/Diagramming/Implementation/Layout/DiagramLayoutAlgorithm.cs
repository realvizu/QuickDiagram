using System.Collections.Generic;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Diagramming.Definition.Layout;
using Codartis.SoftVis.Modeling.Definition;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Implementation.Layout
{
    /// <summary>
    /// Traverses the diagram's node hierarchy, calculates layout for each node that has children
    /// and assembles the results into a <see cref="DiagramLayoutInfo"/>.
    /// </summary>
    public sealed class DiagramLayoutAlgorithm : IDiagramLayoutAlgorithm
    {
        [NotNull] private readonly ILayoutAlgorithmSelectionStrategy _layoutAlgorithmSelectionStrategy;

        /// <summary>
        /// The margin around the rect of the child nodes in the child area of the container nodes.
        /// </summary>
        public double ChildrenAreaMargin { get; }

        public DiagramLayoutAlgorithm(
            [NotNull] ILayoutAlgorithmSelectionStrategy layoutAlgorithmSelectionStrategy,
            double childrenAreaMargin = 10)
        {
            _layoutAlgorithmSelectionStrategy = layoutAlgorithmSelectionStrategy;
            ChildrenAreaMargin = childrenAreaMargin;
        }

        public DiagramLayoutInfo Calculate(IDiagram diagram)
        {
            var layoutStructure = new DiagramLayoutStructure(diagram);
            var rootLayoutAlgorithm = _layoutAlgorithmSelectionStrategy.GetForRoot();
            var rootLayoutInfo = LayoutRecursive(layoutStructure, layoutStructure.RootLayoutGroup, rootLayoutAlgorithm);
            return new DiagramLayoutInfo(rootLayoutInfo.Nodes, rootLayoutInfo.Connectors);
        }

        [NotNull]
        private GroupLayoutInfo LayoutRecursive(
            [NotNull] DiagramLayoutStructure layoutStructure,
            [NotNull] ILayoutGroup layoutGroup,
            [NotNull] IGroupLayoutAlgorithm layoutAlgorithm)
        {
            var childLayoutByParentNodeId = new Dictionary<ModelNodeId, GroupLayoutInfo>();

            foreach (var node in layoutGroup.Nodes)
            {
                var maybeLayoutGroup = layoutStructure.TryGetLayoutGroupByNodeId(node.Id);
                if (!maybeLayoutGroup.HasValue || maybeLayoutGroup.Value.IsEmpty)
                    continue;

                var nodeLayoutAlgorithm = _layoutAlgorithmSelectionStrategy.GetForNode(node);
                var childrenAreaLayoutInfo = LayoutRecursive(layoutStructure, maybeLayoutGroup.Value, nodeLayoutAlgorithm);

                layoutGroup.SetChildrenAreaSize(node.Id, childrenAreaLayoutInfo.Rect.Size);
                childLayoutByParentNodeId.Add(node.Id, childrenAreaLayoutInfo);
            }

            var groupLayoutInfo = layoutAlgorithm.Calculate(layoutGroup);

            foreach (var nodeLayoutInfo in groupLayoutInfo.Nodes)
            {
                if (childLayoutByParentNodeId.TryGetValue(nodeLayoutInfo.Node.Id, out var childrenAreaLayoutInfo))
                    nodeLayoutInfo.ChildrenArea = childrenAreaLayoutInfo;
            }

            return groupLayoutInfo;
        }
    }
}