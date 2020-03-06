using System;
using System.Linq;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Diagramming.Implementation;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling.Definition;
using JetBrains.Annotations;
using MoreLinq;

namespace Codartis.SoftVis.UnitTests.Diagramming
{
    /// <summary>
    /// Test helper for creating diagrams in a more readable way.
    /// Uses names instead of IDs.
    /// </summary>
    public sealed class DiagramBuilder
    {
        private static readonly IModelRelationshipFeatureProvider DummyModelRelationshipFeatureProvider = new DummyModelRelationshipFeatureProvider();

        private readonly DiagramMutator _diagramMutator;

        public DiagramBuilder(
            [NotNull] IModel model,
            IConnectorTypeResolver connectorTypeResolver = null,
            double childrenAreaPadding = 1)
        {
            var diagram = ImmutableDiagram.Create(model, DummyModelRelationshipFeatureProvider);
            _diagramMutator = new DiagramMutator(
                diagram,
                connectorTypeResolver ?? new DummyConnectorTypeResolver(),
                DummyModelRelationshipFeatureProvider,
                childrenAreaPadding);
        }

        [NotNull]
        public IDiagram GetDiagram() => _diagramMutator.GetDiagramEvent().NewDiagram;

        [NotNull]
        public DiagramBuilder AddNode(ModelNodeId nodeId)
        {
            _diagramMutator.AddNode(nodeId);
            return this;
        }

        [NotNull]
        public DiagramBuilder AddNodes([NotNull] params string[] names)
        {
            foreach (var name in names)
                AddNode(name, Size2D.Zero);

            return this;
        }

        [NotNull]
        public DiagramBuilder AddNodes([NotNull] params (string name, Size2D size)[] nodeSpecifications)
        {
            foreach (var nodeSpecification in nodeSpecifications)
                AddNode(nodeSpecification.name, nodeSpecification.size);

            return this;
        }

        [NotNull]
        public DiagramBuilder UpdateNodeTopLeft([NotNull] string nodeName, Point2D nodeTopLeft)
        {
            var modelNode = GetModelNode(nodeName);
            _diagramMutator.UpdateNodeRelativeTopLeft(modelNode.Id, nodeTopLeft);

            return this;
        }

        [NotNull]
        public DiagramBuilder AddConnector([NotNull] params string[] connectorNames)
        {
            foreach (var connectorName in connectorNames)
            {
                var nodeNames = GetNodeNames(connectorName);
                var sourceNodeId = GetModelNode(nodeNames[0]).Id;
                var targetNodeId = GetModelNode(nodeNames[1]).Id;
                var relationship = GetRelationshipByNodeIds(sourceNodeId, targetNodeId);
                _diagramMutator.AddConnector(relationship.Id);
            }

            return this;
        }

        [NotNull]
        public DiagramBuilder UpdateConnectorRoute(string connectorName, Route route)
        {
            var connector = GetDiagramConnector(connectorName);
            _diagramMutator.UpdateConnectorRoute(connector.Id, route);

            return this;
        }

        [NotNull]
        [ItemNotNull]
        private static string[] GetNodeNames([NotNull] string connectorName)
        {
            var nodeNames = connectorName.Split(new[] { "->" }, StringSplitOptions.RemoveEmptyEntries);
            if (nodeNames.Length != 2)
                throw new Exception($"{connectorName} should contain 2 node names.");

            return nodeNames;
        }

        [NotNull]
        public DiagramBuilder AddRelationship(ModelRelationshipId relationshipId)
        {
            _diagramMutator.AddConnector(relationshipId);
            return this;
        }

        [NotNull]
        public DiagramBuilder AddAllModelNodes()
        {
            var modelNodes = GetDiagram().Model.Nodes.ToHashSet();

            while (modelNodes.Any())
            {
                var node = modelNodes.First(i => !HasParentInModel(i.Id) || DiagramContainsItsParent(i.Id));
                AddNode(node.Id);
                modelNodes.Remove(node);
            }

            return this;
        }

        [NotNull]
        public DiagramBuilder AddAllModelRelationships()
        {
            foreach (var modelRelationship in GetDiagram().Model.Relationships.Where(i => i.Stereotype != ModelRelationshipStereotype.Containment))
                AddRelationship(modelRelationship.Id);

            return this;
        }

        [NotNull]
        public DiagramBuilder AddAllModelItems()
        {
            return AddAllModelNodes().AddAllModelRelationships();
        }

        [NotNull]
        public IModelNode GetModelNode([NotNull] string nodeName) => GetDiagram().Model.Nodes.Single(i => i.Name == nodeName);

        [NotNull]
        public IDiagramNode GetDiagramNode([NotNull] string nodeName)
        {
            var modelNodeId = GetModelNode(nodeName).Id;
            return GetDiagram().Nodes.Single(i => i.Id == modelNodeId);
        }

        [NotNull]
        public IDiagramConnector GetDiagramConnector([NotNull] string connectorName)
        {
            var nodeNames = GetNodeNames(connectorName);
            var sourceNode = GetModelNode(nodeNames[0]);
            var targetNode = GetModelNode(nodeNames[1]);
            return GetDiagram().Connectors.Single(i => i.Source == sourceNode.Id && i.Target == targetNode.Id);
        }

        private void AddNode([NotNull] string nodeName, Size2D nodeSize)
        {
            var modelNode = GetModelNode(nodeName);
            _diagramMutator.AddNode(modelNode.Id);
            _diagramMutator.UpdateSize(modelNode.Id, nodeSize);
        }

        [NotNull]
        private IModelRelationship GetRelationshipByNodeIds(ModelNodeId sourceNodeId, ModelNodeId targetNodeId)
            => GetDiagram().Model.Relationships.Single(i => i.Source == sourceNodeId && i.Target == targetNodeId);

        private bool HasParentInModel(ModelNodeId nodeId)
        {
            return GetDiagram().Model.TryGetParentNode(nodeId).HasValue;
        }

        private bool DiagramContainsItsParent(ModelNodeId nodeId)
        {
            var maybeParent = GetDiagram().Model.TryGetParentNode(nodeId);
            return maybeParent.HasValue && GetDiagram().TryGetNode(maybeParent.Value.Id).HasValue;
        }

    }
}