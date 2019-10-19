using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Diagramming.Definition.Layout;
using Codartis.SoftVis.Diagramming.Implementation.Layout;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.UnitTests.Modeling;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;

namespace Codartis.SoftVis.UnitTests.Diagramming.Implementation.Layout
{
    public sealed class DiagramLayoutAlgorithmTests
    {
        private const double ChildrenAreaPadding = 1;

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
                .AddNodes(("A", payloadAreaSize: (1, 1)), ("B", payloadAreaSize: (2, 2)));
                //.AddAllModelRelationships();

            SetUpRootLayoutInfo(diagramBuilder, ("A", topLeft: (0, 0)), ("B", topLeft: (1, 1)));

            var diagramLayoutInfo = CreateLayoutAlgorithm().Calculate(diagramBuilder.Diagram);

            var diagramNodeA = diagramBuilder.GetDiagramNode("A");
            var diagramNodeB = diagramBuilder.GetDiagramNode("B");

            diagramLayoutInfo.Boxes.Should().BeEquivalentTo(
                new BoxLayoutInfo(diagramNodeA.ShapeId, topLeft: (0, 0), payloadAreaSize: (1, 1), childrenAreaSize: (0, 0)),
                new BoxLayoutInfo(diagramNodeB.ShapeId, topLeft: (1, 1), payloadAreaSize: (2,2), childrenAreaSize: (0, 0))
                );
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
                .AddChildNodes("B", ("C", payloadAreaSize: (3, 3)), ("D", payloadAreaSize: (4, 4)));
                //.AddAllModelRelationships();

            SetUpRootLayoutInfo(diagramBuilder, ("A", topLeft: (0, 0)), ("B", topLeft: (1, 1)));
            SetUpNodeLayoutInfo(diagramBuilder, "B", ("C", topLeft: (1, 1)), ("D", topLeft: (2, 2)));

            var diagramLayoutInfo = CreateLayoutAlgorithm().Calculate(diagramBuilder.Diagram);

            var diagramNodeA = diagramBuilder.GetDiagramNode("A");
            var diagramNodeB = diagramBuilder.GetDiagramNode("B");
            var diagramNodeC = diagramBuilder.GetDiagramNode("C");
            var diagramNodeD = diagramBuilder.GetDiagramNode("D");

            diagramLayoutInfo.Should().BeEquivalentTo(
                new GroupLayoutInfo(
                    new[]
                    {
                        new BoxLayoutInfo(diagramNodeA.ShapeId, topLeft: (0, 0), payloadAreaSize: (1, 1), childrenAreaSize: (0, 0)),
                        new BoxLayoutInfo(
                            diagramNodeB.ShapeId,
                            topLeft: (1, 1),
                            payloadAreaSize: (2, 2),
                            childrenAreaSize: (7, 7),
                            new GroupLayoutInfo(
                                new[]
                                {
                                    new BoxLayoutInfo(diagramNodeC.ShapeId, topLeft: (2, 4), payloadAreaSize: (3, 3), childrenAreaSize: (0, 0)),
                                    new BoxLayoutInfo(diagramNodeD.ShapeId, topLeft: (3, 5), payloadAreaSize: (4, 4), childrenAreaSize: (0, 0)),
                                }
                            )
                        ),
                    }
                ));
        }

        private void SetUpRootLayoutInfo(
            [NotNull] DiagramBuilder diagramBuilder,
            [NotNull] params (string nodeName, Point2D topLeft)[] nodeLayoutSpecifications)
            => SetUpNodeLayoutInfo(diagramBuilder, targetNodeName: null, nodeLayoutSpecifications);

        private void SetUpNodeLayoutInfo(
            [NotNull] DiagramBuilder diagramBuilder,
            [CanBeNull] string targetNodeName,
            [NotNull] params (string nodeName, Point2D topLeft)[] nodeLayoutSpecifications)
        {
            var expectedLayoutInfo = CreateLayoutInfo(diagramBuilder, nodeLayoutSpecifications);

            var layoutAlgorithm = new TestLayoutAlgorithm(expectedLayoutInfo);

            if (targetNodeName == null)
                _layoutAlgorithmSelectionStrategy.SetLayoutAlgorithmForRoot(layoutAlgorithm);
            else
                _layoutAlgorithmSelectionStrategy.SetLayoutAlgorithmForNode(diagramBuilder.GetDiagramNode(targetNodeName), layoutAlgorithm);
        }

        private static LayoutInfo CreateLayoutInfo(
            DiagramBuilder diagramBuilder,
            [NotNull] params (string nodeName, Point2D topLeft)[] nodeLayoutSpecifications)
        {
            return new LayoutInfo(
                nodeLayoutSpecifications
                    .ToDictionary(
                        i => diagramBuilder.GetDiagramNode(i.nodeName).Id,
                        i => new Rect2D(i.topLeft, diagramBuilder.GetDiagramNode(i.nodeName).Size)
                    ),
                new Dictionary<ModelRelationshipId, Route>()
            );
        }

        [NotNull]
        private IDiagramLayoutAlgorithm CreateLayoutAlgorithm()
        {
            return new DiagramLayoutAlgorithm(_layoutAlgorithmSelectionStrategy, ChildrenAreaPadding);
        }
    }
}