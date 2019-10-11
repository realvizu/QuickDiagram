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
                .AddNodes(("A", payloadAreaSize: (1, 1)), ("B", payloadAreaSize: (2, 2)))
                .AddAllModelRelationships();

            var rootLayoutInfo = SetUpRootLayoutInfo(diagramBuilder, ("A", topLeft: (0, 0)), ("B", topLeft: (1, 1)));

            var diagramLayoutInfo = CreateLayoutAlgorithm().Calculate(diagramBuilder.Diagram);

            diagramLayoutInfo.Boxes.Should().BeEquivalentTo(rootLayoutInfo.Boxes);
        }

        /// <summary>
        /// A->B[C->D]
        /// </summary>
        [Fact]
        public void Calculate_WithChildNodes_Works()
        {
            var model = _modelBuilder
                .AddNodes("A", "B")
                .AddChildNodes("B", "C", "D")
                .AddRelationships("A->B", "C->D")
                .Model;

            var diagramBuilder = new DiagramBuilder(model)
                .AddNodes(("A", payloadAreaSize: (1, 1)), ("B", payloadAreaSize: (2, 2)))
                .AddChildNodes("B", ("C", payloadAreaSize: (3, 3)), ("D", payloadAreaSize: (4, 4)))
                .AddAllModelRelationships();

            SetUpRootLayoutInfo(diagramBuilder, ("A", topLeft: (0, 0)), ("B", topLeft: (1, 1)));
            SetUpNodeLayoutInfo(diagramBuilder, "B", ("C", topLeft: (1, 1)), ("D", topLeft: (2, 2)));

            var diagramLayoutInfo = CreateLayoutAlgorithm().Calculate(diagramBuilder.Diagram);

            diagramLayoutInfo.Should().BeEquivalentTo(
                new GroupLayoutInfo(
                    new[]
                    {
                        new BoxLayoutInfo(diagramBuilder.GetDiagramNode("A"), topLeft: (0, 0)),
                        new BoxLayoutInfo(
                            diagramBuilder.GetDiagramNode("B"),
                            topLeft: (1, 1),
                            new GroupLayoutInfo(
                                new[]
                                {
                                    new BoxLayoutInfo(diagramBuilder.GetDiagramNode("C"), topLeft: (1, 3)),
                                    new BoxLayoutInfo(diagramBuilder.GetDiagramNode("D"), topLeft: (2, 4)),
                                })),
                    }
                ));
        }

        [NotNull]
        private GroupLayoutInfo SetUpRootLayoutInfo(
            [NotNull] DiagramBuilder diagramBuilder,
            [NotNull] params (string nodeName, Point2D topLeft )[] nodeLayoutSpecifications)
            => SetUpNodeLayoutInfo(diagramBuilder, targetNodeName: null, nodeLayoutSpecifications);

        [NotNull]
        private GroupLayoutInfo SetUpNodeLayoutInfo(
            [NotNull] DiagramBuilder diagramBuilder,
            [CanBeNull] string targetNodeName,
            [NotNull] params (string nodeName, Point2D topLeft)[] nodeLayoutSpecifications)
        {
            var expectedLayoutInfo = CreateGroupLayoutInfo(diagramBuilder, nodeLayoutSpecifications);

            var layoutAlgorithm = new TestLayoutAlgorithm(expectedLayoutInfo);

            if (targetNodeName == null)
                _layoutAlgorithmSelectionStrategy.SetLayoutAlgorithmForRoot(layoutAlgorithm);
            else
                _layoutAlgorithmSelectionStrategy.SetLayoutAlgorithmForNode(diagramBuilder.GetDiagramNode(targetNodeName), layoutAlgorithm);

            return expectedLayoutInfo;
        }

        [NotNull]
        private static GroupLayoutInfo CreateGroupLayoutInfo(
            DiagramBuilder diagramBuilder,
            [NotNull] params (string nodeName, Point2D topLeft)[] nodeLayoutSpecifications)
        {
            return new GroupLayoutInfo(
                nodeLayoutSpecifications
                    .Select(i => new BoxLayoutInfo(diagramBuilder.GetDiagramNode(i.nodeName), i.topLeft))
                    .ToList());
        }

        [NotNull]
        private IDiagramLayoutAlgorithm CreateLayoutAlgorithm()
        {
            return new DiagramLayoutAlgorithm(_layoutAlgorithmSelectionStrategy, childrenAreaPadding: 0);
        }
    }
}