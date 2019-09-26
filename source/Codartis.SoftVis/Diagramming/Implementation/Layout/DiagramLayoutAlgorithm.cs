using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Diagramming.Definition.Layout;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Implementation.Layout
{
    public sealed class DiagramLayoutAlgorithm : IDiagramLayoutAlgorithm
    {
        [NotNull] private readonly ILayoutAlgorithmSelectionStrategy _layoutAlgorithmSelectionStrategy;

        /// <summary>
        /// The margin around the rect of the child nodes in the child area of container nodes.
        /// </summary>
        public double ChildAreaMargin { get; }

        public DiagramLayoutAlgorithm(
            [NotNull] ILayoutAlgorithmSelectionStrategy layoutAlgorithmSelectionStrategy,
            double childAreaMargin = 10)
        {
            _layoutAlgorithmSelectionStrategy = layoutAlgorithmSelectionStrategy;
            ChildAreaMargin = childAreaMargin;
        }

        public DiagramLayoutInfo Calculate(IDiagram diagram)
        {
            var layoutStructure = new DiagramLayoutStructure(diagram);
            var rootLayoutAlgorithm = _layoutAlgorithmSelectionStrategy.GetForRoot();
            var updatedDiagram = LayoutRecursive(diagram, layoutStructure, layoutStructure.RootLayoutGroup, rootLayoutAlgorithm);
            //return updatedDiagram.GetLayoutSpecification();
            return new DiagramLayoutInfo();
        }

        [NotNull]
        private IDiagram LayoutRecursive(
            [NotNull] IDiagram diagram,
            [NotNull] DiagramLayoutStructure layoutStructure,
            [NotNull] ILayoutGroup layoutGroup,
            [NotNull] IGroupLayoutAlgorithm layoutAlgorithm)
        {
            foreach (var node in layoutGroup.Nodes)
            {
                var maybeLayoutGroup = layoutStructure.TryGetLayoutGroupByNodeId(node.Id);
                if (!maybeLayoutGroup.HasValue)
                    continue;

                var nodeLayoutAlgorithm = _layoutAlgorithmSelectionStrategy.GetForNode(node);
                diagram = LayoutRecursive(diagram, layoutStructure, maybeLayoutGroup.Value, nodeLayoutAlgorithm);

                var updatedLayoutGroup = layoutStructure.TryGetLayoutGroupByNodeId(node.Id).Value;
                diagram = diagram.UpdateNode(node.WithChildrenAreaSize(updatedLayoutGroup.Rect.Size));
            }

            var layout = layoutAlgorithm.Calculate(layoutGroup);
            diagram = diagram.ApplyLayout(layout);
            return diagram;
        }
    }
}