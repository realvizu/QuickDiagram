using System.Linq;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Diagramming.Definition.Layout;
using Codartis.SoftVis.Diagramming.Implementation.Layout;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.UnitTests.Modeling;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;

namespace Codartis.SoftVis.UnitTests.Diagramming.Implementation.Layout
{
    public sealed class DiagramLayoutAlgorithmTests
    {
        [NotNull] private readonly ModelBuilder _modelBuilder;
        [NotNull] private readonly TestLayoutAlgorithmSelectionStrategy _layoutAlgorithmSelectionStrategy;

        public DiagramLayoutAlgorithmTests()
        {
            _modelBuilder = new ModelBuilder();

            _layoutAlgorithmSelectionStrategy = new TestLayoutAlgorithmSelectionStrategy();
        }

        /// <summary>
        /// A->B
        /// </summary>
        [Fact]
        public void Calculate_RootNodesOnly_Works()
        {
            var model = _modelBuilder
                .AddNodes("A", "B")
                .AddRelationships("A->B")
                .Model;

            var diagramBuilder = new DiagramBuilder(model)
                .AddNodes(("A", 1, 1), ("B", 2, 2))
                .AddAllModelRelationships();

            var rootLayoutInfo = SetUpRootLayoutInfo(diagramBuilder, ("A", 0, 0), ("B", 1, 1));

            var diagramLayoutInfo = CreateLayoutAlgorithm().Calculate(diagramBuilder.Diagram);

            diagramLayoutInfo.RootNodes.Should().BeEquivalentTo(rootLayoutInfo.Nodes);
        }

        /// <summary>
        /// A->B[C->D]
        /// </summary>
        [Fact]
        public void Calculate_WithChildrenArea_Works()
        {
            var model = _modelBuilder
                .AddNodes("A", "B")
                .AddRelationships("A->B")
                .AddChildNodes("B", "C", "D")
                .AddRelationships("C->D")
                .Model;

            var diagramBuilder = new DiagramBuilder(model)
                .AddNodes(("A", 1, 1), ("B", 2, 2))
                .AddChildNodes("B", ("C", 3, 3), ("D", 4, 4))
                .AddAllModelRelationships();

            var rootLayoutInfo = SetUpRootLayoutInfo(diagramBuilder, ("A", 0, 0), ("B", 1, 1));
            var nodeBLayoutInfo = SetUpNodeLayoutInfo(diagramBuilder, "B", ("C", 0, 0), ("D", 1, 1));

            var diagramLayoutInfo = CreateLayoutAlgorithm().Calculate(diagramBuilder.Diagram);

            diagramLayoutInfo.RootNodes.Should().BeEquivalentTo(rootLayoutInfo.Nodes);

            var nodeBResult = diagramLayoutInfo.RootNodes.First(i => i.Node.Name == "B");
            nodeBResult.ChildrenArea.Nodes.Should().BeEquivalentTo(nodeBLayoutInfo.Nodes);

            // C+D rect should be: from C.TopLeft(0,0) to D.TopLeft(1,1) + D.PayloadAreaSize(4,4)
            nodeBResult.ChildrenArea.Rect.Should().Be(new Rect2D(0, 0, 5, 5));

            // B rect should be B.TopLeft(1,1) + B.Size(5,7) which comes from stacking B.PayloadAreaSize(2,2) and B.ChildrenAreaSize(5,5)
            nodeBResult.Rect.Should().Be(new Rect2D(1, 1, 6, 8));
        }

        [NotNull]
        private GroupLayoutInfo SetUpRootLayoutInfo(
            [NotNull] DiagramBuilder diagramBuilder,
            [NotNull] params (string nodeName, double x, double y)[] nodeLayoutSpecifications)
            => SetUpNodeLayoutInfo(diagramBuilder, targetNodeName: null, nodeLayoutSpecifications);

        [NotNull]
        private GroupLayoutInfo SetUpNodeLayoutInfo(
            [NotNull] DiagramBuilder diagramBuilder,
            [CanBeNull] string targetNodeName,
            [NotNull] params (string nodeName, double x, double y)[] nodeLayoutSpecifications)
        {
            var expectedLayoutInfo = new GroupLayoutInfo(
                nodeLayoutSpecifications
                    .Select(i => new NodeLayoutInfo(diagramBuilder.GetDiagramNode(i.nodeName), new Point2D(i.x, i.y)))
                    .ToList());

            var layoutAlgorithm = new TestLayoutAlgorithm(expectedLayoutInfo);

            if (targetNodeName == null)
                _layoutAlgorithmSelectionStrategy.SetLayoutAlgorithmForRoot(layoutAlgorithm);
            else
                _layoutAlgorithmSelectionStrategy.SetLayoutAlgorithmForNode(diagramBuilder.GetDiagramNode(targetNodeName), layoutAlgorithm);

            return expectedLayoutInfo;
        }

        [NotNull]
        private IDiagramLayoutAlgorithm CreateLayoutAlgorithm()
        {
            return new DiagramLayoutAlgorithm(_layoutAlgorithmSelectionStrategy, childrenAreaMargin: 0);
        }
    }
}