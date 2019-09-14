using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Diagramming.Definition.Layout;
using Codartis.SoftVis.Diagramming.Implementation;
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
            var layoutGroup = LayoutGroup.Empty;
            var layoutAlgorithm = CreateLayoutAlgorithm();
            var layout = layoutAlgorithm.Calculate(layoutGroup);
            layout.ConnectorRoutes.Should().BeEmpty();
            layout.NodeTopLeftPositions.Should().BeEmpty();
        }

        [Fact]
        public void Calculate_Nodes_Works()
        {
            var layoutGroup = LayoutGroup.Create(
                CreateNodes(
                    new NodeSpecification("A", new Size2D(1, 2)),
                    new NodeSpecification("B", new Size2D(3, 4)),
                    new NodeSpecification("C", new Size2D(5, 6))),
                ImmutableHashSet<IDiagramConnector>.Empty);

            var layoutAlgorithm = CreateLayoutAlgorithm();
            var layout = layoutAlgorithm.Calculate(layoutGroup);

            layout.ConnectorRoutes.Should().BeEmpty();
            layout.NodeTopLeftPositions.Should().BeEquivalentTo(
                CreateNodeIdToPointDictionary(layoutGroup, new Point2D(0, 0), new Point2D(0, 4), new Point2D(0, 10)));
        }

        [Fact]
        public void Calculate_Connectors_Works()
        {
            var nodes = CreateNodes(
                new NodeSpecification("A", new Size2D(1, 2)),
                new NodeSpecification("B", new Size2D(3, 4)));

            var connectors = CreateConnectors(nodes, new ConnectorSpecification("A", "B"));

            var layoutGroup = LayoutGroup.Create(nodes, connectors);
            var layoutAlgorithm = CreateLayoutAlgorithm();
            var layout = layoutAlgorithm.Calculate(layoutGroup);

            var connectorIdToRouteDictionary = CreateConnectorIdToRouteDictionary(layoutGroup, new Route(new Point2D(0.5, 1), new Point2D(1.5, 6)));
            layout.ConnectorRoutes.Should().BeEquivalentTo(connectorIdToRouteDictionary);
        }

        [NotNull]
        private static IDictionary<ModelNodeId, Point2D> CreateNodeIdToPointDictionary([NotNull] ILayoutGroup layoutGroup, [NotNull] params Point2D[] points)
        {
            return layoutGroup.Nodes
                .OrderBy(i => i.ModelNode.Name)
                .Select(i => i.ModelNode.Id)
                .Zip(points, (i, j) => new KeyValuePair<ModelNodeId, Point2D>(i, j))
                .ToDictionary(i => i.Key, i => i.Value);
        }

        [NotNull]
        private static IDictionary<ModelRelationshipId, Route> CreateConnectorIdToRouteDictionary(
            [NotNull] ILayoutGroup layoutGroup,
            [NotNull] params Route[] routes)
        {
            return layoutGroup.Connectors
                .OrderBy(i => (string)i.ModelRelationship.Payload)
                .Select(i => i.ModelRelationship.Id)
                .Zip(routes, (i, j) => new KeyValuePair<ModelRelationshipId, Route>(i, j))
                .ToDictionary(i => i.Key, i => i.Value);
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
        private static ILayoutAlgorithm CreateLayoutAlgorithm() => new VerticalNodeLayoutAlgorithm(gapBetweenNodes: 2);

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