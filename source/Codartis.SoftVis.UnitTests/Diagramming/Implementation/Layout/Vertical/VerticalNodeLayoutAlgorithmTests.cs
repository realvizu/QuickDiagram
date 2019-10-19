using System;
using System.Collections.Immutable;
using System.Linq;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Diagramming.Definition.Layout;
using Codartis.SoftVis.Diagramming.Implementation;
using Codartis.SoftVis.Diagramming.Implementation.Layout;
using Codartis.SoftVis.Diagramming.Implementation.Layout.Vertical;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.Modeling.Implementation;
using Codartis.Util;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;

namespace Codartis.SoftVis.UnitTests.Diagramming.Implementation.Layout.Vertical
{
    public class VerticalNodeLayoutAlgorithmTests
    {
        [Fact]
        public void Calculate_EmptyLayoutGroup_Works()
        {
            var layoutGroup = new LayoutGroup();

            var layout = CreateLayoutAlgorithm().Calculate(layoutGroup);

            layout.VertexRects.Should().BeEmpty();
            layout.EdgeRoutes.Should().BeEmpty();
        }

        [Fact]
        public void Calculate_Nodes_Works()
        {
            var layoutGroup = LayoutGroup.CreateForRoot(
                CreateNodes(
                    new NodeSpecification("A", new Size2D(1, 2)),
                    new NodeSpecification("B", new Size2D(3, 4)),
                    new NodeSpecification("C", new Size2D(5, 6))),
                ImmutableHashSet<IDiagramConnector>.Empty);

            var layout = CreateLayoutAlgorithm().Calculate(layoutGroup);

            layout.VertexRects.Values.Select(i => i.TopLeft)
                .Should().BeEquivalentTo(new Point2D(0, 0), new Point2D(0, 4), new Point2D(0, 10));

            layout.EdgeRoutes.Should().BeEmpty();
        }

        [Fact]
        public void Calculate_Connectors_Works()
        {
            var nodes = CreateNodes(
                new NodeSpecification("A", new Size2D(1, 2)),
                new NodeSpecification("B", new Size2D(3, 4)));

            var connectors = CreateConnectors(nodes, new ConnectorSpecification("A", "B"));

            var layoutGroup = LayoutGroup.CreateForRoot(nodes, connectors);
            var layoutAlgorithm = CreateLayoutAlgorithm();
            var layout = layoutAlgorithm.Calculate(layoutGroup);

            layout.EdgeRoutes.Values.Should().BeEquivalentTo(new[] { new Route(new Point2D(0.5, 1), new Point2D(1.5, 6)) });
        }

        [NotNull]
        [ItemNotNull]
        private static IImmutableSet<IDiagramNode> CreateNodes([NotNull] params NodeSpecification[] nodeSpecifications)
        {
            return nodeSpecifications
                .Select(i => new DiagramNode(CreateModelNode(i.Name), DateTime.Now, Point2D.Undefined, i.Size, Size2D.Zero, Maybe<ModelNodeId>.Nothing))
                .OfType<IDiagramNode>()
                .ToImmutableHashSet();
        }

        [NotNull]
        private static IModelNode CreateModelNode([NotNull] string name)
        {
            return new ModelNode(ModelNodeId.Create(), name, ModelNodeStereotype.Default);
        }

        [NotNull]
        [ItemNotNull]
        private static IImmutableSet<IDiagramConnector> CreateConnectors(
            [NotNull] IImmutableSet<IDiagramNode> nodes,
            [NotNull] params ConnectorSpecification[] connectorSpecifications)
        {
            return connectorSpecifications.Select(i => CreateConnector(nodes, i.SourceName, i.TargetName)).ToImmutableHashSet();
        }

        [NotNull]
        private static IDiagramConnector CreateConnector(
            [NotNull] IImmutableSet<IDiagramNode> nodes,
            [NotNull] string sourceNodeName,
            [NotNull] string targetNodeName)
        {
            var sourceNode = nodes.Single(i => i.Name == sourceNodeName);
            var targetNode = nodes.Single(i => i.Name == targetNodeName);
            var relationshipName = $"{sourceNodeName}->{targetNodeName}";

            var relationship = new ModelRelationship(
                ModelRelationshipId.Create(),
                sourceNode.Id,
                targetNode.Id,
                ModelRelationshipStereotype.Default,
                relationshipName);

            return new DiagramConnector(relationship, ConnectorTypes.Dependency);
        }

        [NotNull]
        private static IGroupLayoutAlgorithm CreateLayoutAlgorithm() => new VerticalNodeLayoutAlgorithm(gapBetweenNodes: 2);

        private struct NodeSpecification
        {
            public string Name { get; }
            public Size2D Size { get; }

            public NodeSpecification(string name, Size2D size)
            {
                Name = name;
                Size = size;
            }
        }

        private struct ConnectorSpecification
        {
            public string SourceName { get; }
            public string TargetName { get; }

            public ConnectorSpecification(string sourceName, string targetName)
            {
                SourceName = sourceName;
                TargetName = targetName;
            }
        }
    }
}