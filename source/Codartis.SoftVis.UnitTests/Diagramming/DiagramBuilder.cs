using System;
using System.Linq;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.Util;
using JetBrains.Annotations;
using MoreLinq;

namespace Codartis.SoftVis.UnitTests.Diagramming
{
    /// <summary>
    /// Test helper for creating diagrams in a more readable way.
    /// Ignores events returned by diagram mutators. Uses names instead of IDs.
    /// </summary>
    public sealed class DiagramBuilder
    {
        [NotNull] public IDiagram Diagram { get; private set; }

        public DiagramBuilder([NotNull] IModel model, IConnectorTypeResolver connectorTypeResolver = null)
        {
            Diagram = SoftVis.Diagramming.Implementation.Diagram.Create(model, connectorTypeResolver ?? new DummyConnectorTypeResolver());
        }

        public DiagramBuilder([NotNull] IDiagram diagram)
        {
            Diagram = diagram;
        }

        [NotNull]
        public DiagramBuilder AddNode(ModelNodeId nodeId, ModelNodeId? parentNodeId = null)
        {
            Diagram = Diagram.AddNode(nodeId, parentNodeId).NewDiagram;
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
        public DiagramBuilder AddNodes([NotNull] params (string name, Size2D payloadAreaSize)[] nodeSpecifications)
        {
            foreach (var nodeSpecification in nodeSpecifications)
                AddNode(nodeSpecification.name, nodeSpecification.payloadAreaSize);

            return this;
        }

        [NotNull]
        public DiagramBuilder AddChildNodes(
            [NotNull] string parentNodeName,
            [NotNull] params (string name, Size2D payloadAreaSize)[] childNodeSpecifications)
        {
            foreach (var childNodeSpecification in childNodeSpecifications)
                AddNode(childNodeSpecification.name, childNodeSpecification.payloadAreaSize, parentNodeName);

            return this;
        }

        [NotNull]
        public DiagramBuilder UpdateNodeTopLeft([NotNull] string nodeName, Point2D nodeTopLeft)
        {
            var modelNode = GetModelNode(nodeName);
            Diagram = Diagram.UpdateNodeTopLeft(modelNode.Id, nodeTopLeft).NewDiagram;

            return this;
        }

        [NotNull]
        public DiagramBuilder UpdateChildrenAreaSize([NotNull] string nodeName, double width, double height)
        {
            var modelNode = GetModelNode(nodeName);
            Diagram = Diagram.UpdateNodeChildrenAreaSize(modelNode.Id, (width, height)).NewDiagram;

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
                Diagram = Diagram.AddConnector(relationship.Id).NewDiagram;
            }

            return this;
        }

        [NotNull]
        public DiagramBuilder UpdateConnectorRoute(string connectorName, Route route)
        {
            var connector = GetDiagramConnector(connectorName);
            Diagram = Diagram.UpdateConnectorRoute(connector.Id, route).NewDiagram;

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
            Diagram = Diagram.AddConnector(relationshipId).NewDiagram;
            return this;
        }

        [NotNull]
        public DiagramBuilder AddAllModelNodes()
        {
            var modelNodes = Diagram.Model.Nodes.ToHashSet();

            while (modelNodes.Any())
            {
                var node = modelNodes.First(i => !HasParentInModel(i.Id) || DiagramContainsItsParent(i.Id));
                var maybeParentNode = Diagram.Model.TryGetParentNode(node.Id);
                AddNode(node.Id, maybeParentNode.FromMaybe()?.Id);
                modelNodes.Remove(node);
            }

            return this;
        }

        [NotNull]
        public DiagramBuilder AddAllModelRelationships()
        {
            foreach (var modelRelationship in Diagram.Model.Relationships.Where(i => i.Stereotype != ModelRelationshipStereotype.Containment))
                AddRelationship(modelRelationship.Id);

            return this;
        }

        [NotNull]
        public DiagramBuilder AddAllModelItems()
        {
            return AddAllModelNodes().AddAllModelRelationships();
        }

        [NotNull]
        public IModelNode GetModelNode([NotNull] string nodeName) => Diagram.Model.Nodes.Single(i => i.Name == nodeName);

        [NotNull]
        public IDiagramNode GetDiagramNode([NotNull] string nodeName)
        {
            var modelNodeId = GetModelNode(nodeName).Id;
            return Diagram.Nodes.Single(i => i.Id == modelNodeId);
        }

        [NotNull]
        public IDiagramConnector GetDiagramConnector([NotNull] string connectorName)
        {
            var nodeNames = GetNodeNames(connectorName);
            var sourceNode = GetModelNode(nodeNames[0]);
            var targetNode = GetModelNode(nodeNames[1]);
            return Diagram.Connectors.Single(i => i.Source == sourceNode.Id && i.Target == targetNode.Id);
        }

        private void AddNode([NotNull] string nodeName, Size2D nodePayloadAreaSize, [CanBeNull] string parentNodeName = null)
        {
            var parentNodeId = parentNodeName == null
                ? (ModelNodeId?)null
                : GetModelNode(parentNodeName).Id;

            var modelNode = GetModelNode(nodeName);
            Diagram = Diagram.AddNode(modelNode.Id, parentNodeId).NewDiagram;

            Diagram = Diagram.UpdateNodePayloadAreaSize(modelNode.Id, nodePayloadAreaSize).NewDiagram;
        }

        [NotNull]
        private IModelRelationship GetRelationshipByNodeIds(ModelNodeId sourceNodeId, ModelNodeId targetNodeId)
            => Diagram.Model.Relationships.Single(i => i.Source == sourceNodeId && i.Target == targetNodeId);

        private bool HasParentInModel(ModelNodeId nodeId)
        {
            return Diagram.Model.TryGetParentNode(nodeId).HasValue;
        }

        private bool DiagramContainsItsParent(ModelNodeId nodeId)
        {
            var maybeParent = Diagram.Model.TryGetParentNode(nodeId);
            return maybeParent.HasValue && Diagram.TryGetNode(maybeParent.Value.Id).HasValue;
        }

        /// <summary>
        /// A dummy resolver that always returns Dependency connector type.
        /// </summary>
        private sealed class DummyConnectorTypeResolver : IConnectorTypeResolver
        {
            public ConnectorType GetConnectorType(ModelRelationshipStereotype stereotype) => ConnectorTypes.Dependency;
        }
    }
}