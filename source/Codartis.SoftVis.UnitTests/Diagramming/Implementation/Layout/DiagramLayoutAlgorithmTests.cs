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
                .AddNodes(("A", payloadAreaSize: (1, 1)), ("B", payloadAreaSize: (2, 2)))
                .AddAllModelRelationships();

            SetUpRootLayoutInfo(
                diagramBuilder,
                new[] { ("A", topLeft: new Point2D(0, 0)), ("B", topLeft: (1, 1)) },
                new[] { ("A->B", new Route((0.5, 0.5), (1.5, 1.5))) }
            );

            var diagramLayoutInfo = CreateLayoutAlgorithm().Calculate(diagramBuilder.Diagram);

            var diagramNodeA = diagramBuilder.GetDiagramNode("A");
            var diagramNodeB = diagramBuilder.GetDiagramNode("B");
            var diagramConnector = diagramBuilder.GetDiagramConnector("A->B");

            diagramLayoutInfo.Boxes.Should().BeEquivalentTo(
                new BoxLayoutInfo(diagramNodeA.ShapeId, topLeft: (0, 0), payloadAreaSize: (1, 1), childrenAreaSize: (0, 0)),
                new BoxLayoutInfo(diagramNodeB.ShapeId, topLeft: (1, 1), payloadAreaSize: (2, 2), childrenAreaSize: (0, 0))
            );
            diagramLayoutInfo.Lines.Should().BeEquivalentTo(
                new LineLayoutInfo(diagramConnector.ShapeId, new Route((0.5, 0.5), (1.5, 1.5)))
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
                .AddChildNodes("B", ("C", payloadAreaSize: (3, 3)), ("D", payloadAreaSize: (4, 4)))
                .AddAllModelRelationships();

            SetUpRootLayoutInfo(
                diagramBuilder,
                new[] { ("A", topLeft: new Point2D(0, 0)), ("B", topLeft: (2, 0)) },
                new[] { ("A->B", new Route((1, 1), (2, 2))) }
            );
            SetUpNodeLayoutInfo(
                diagramBuilder,
                "B",
                new[] { ("C", topLeft: new Point2D(0, 0)), ("D", topLeft: (4, 1)) },
                new[] { ("C->D", new Route((3, 1.5), (4, 3))) }
            );

            var diagramLayoutInfo = CreateLayoutAlgorithm().Calculate(diagramBuilder.Diagram);

            var diagramNodeA = diagramBuilder.GetDiagramNode("A");
            var diagramNodeB = diagramBuilder.GetDiagramNode("B");
            var diagramNodeC = diagramBuilder.GetDiagramNode("C");
            var diagramNodeD = diagramBuilder.GetDiagramNode("D");
            var diagramConnectorAtoB = diagramBuilder.GetDiagramConnector("A->B");
            var diagramConnectorCtoD = diagramBuilder.GetDiagramConnector("C->D");

            diagramLayoutInfo.Should().BeEquivalentTo(
                new GroupLayoutInfo(
                    new[]
                    {
                        new BoxLayoutInfo(diagramNodeA.ShapeId, topLeft: (0, 0), payloadAreaSize: (1, 1), childrenAreaSize: (0, 0)),
                        new BoxLayoutInfo(
                            diagramNodeB.ShapeId,
                            topLeft: (2, 0),
                            payloadAreaSize: (2, 2),
                            childrenAreaSize: (10, 7),
                            new GroupLayoutInfo(
                                new[]
                                {
                                    new BoxLayoutInfo(diagramNodeC.ShapeId, topLeft: (3, 3), payloadAreaSize: (3, 3), childrenAreaSize: (0, 0)),
                                    new BoxLayoutInfo(diagramNodeD.ShapeId, topLeft: (7, 4), payloadAreaSize: (4, 4), childrenAreaSize: (0, 0)),
                                },
                                new[]
                                {
                                    new LineLayoutInfo(diagramConnectorCtoD.ShapeId, new Route((6, 4.5), (7, 6)))
                                }
                            )
                        ),
                    },
                    new[]
                    {
                        new LineLayoutInfo(diagramConnectorAtoB.ShapeId, new Route((1, 1), (2, 2)))
                    }
                ));
        }

        private void SetUpRootLayoutInfo(
            [NotNull] DiagramBuilder diagramBuilder,
            (string nodeName, Point2D topLeft)[] nodeLayoutSpecifications = null,
            (string connectorName, Route route)[] connectorRouteSpecifications = null)
            => SetUpNodeLayoutInfo(diagramBuilder, targetNodeName: null, nodeLayoutSpecifications, connectorRouteSpecifications);

        private void SetUpNodeLayoutInfo(
            [NotNull] DiagramBuilder diagramBuilder,
            [CanBeNull] string targetNodeName,
            (string nodeName, Point2D topLeft)[] nodeLayoutSpecifications = null,
            (string connectorName, Route route)[] connectorRouteSpecifications = null)
        {
            var expectedLayoutInfo = CreateLayoutInfo(diagramBuilder, nodeLayoutSpecifications, connectorRouteSpecifications);
            var layoutAlgorithm = new TestLayoutAlgorithm(expectedLayoutInfo);

            if (targetNodeName == null)
                _layoutAlgorithmSelectionStrategy.SetLayoutAlgorithmForRoot(layoutAlgorithm);
            else
                _layoutAlgorithmSelectionStrategy.SetLayoutAlgorithmForNode(diagramBuilder.GetDiagramNode(targetNodeName), layoutAlgorithm);
        }

        private static LayoutInfo CreateLayoutInfo(
            DiagramBuilder diagramBuilder,
            (string nodeName, Point2D topLeft)[] nodeLayoutSpecifications = null,
            (string connectorName, Route route)[] connectorRouteSpecifications = null)
        {
            return new LayoutInfo(
                nodeLayoutSpecifications?
                    .ToDictionary(
                        i => diagramBuilder.GetDiagramNode(i.nodeName).Id,
                        i => new Rect2D(i.topLeft, diagramBuilder.GetDiagramNode(i.nodeName).Size)
                    ) ??
                new Dictionary<ModelNodeId, Rect2D>(),
                connectorRouteSpecifications?
                    .ToDictionary(
                        i => diagramBuilder.GetDiagramConnector(i.connectorName).Id,
                        i => i.route
                    ) ??
                new Dictionary<ModelRelationshipId, Route>()
            );
        }

        [NotNull]
        private IDiagramLayoutAlgorithm CreateLayoutAlgorithm()
        {
            return new DiagramLayoutAlgorithm(
                _layoutAlgorithmSelectionStrategy,
                new TestConnectorRoutingAlgorithm(),
                ChildrenAreaPadding);
        }
    }
}