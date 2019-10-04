using System;
using System.Linq;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Diagramming.Implementation;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling.Definition;
using JetBrains.Annotations;

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
        public DiagramBuilder AddNode(ModelNodeId nodeId)
        {
            Diagram = Diagram.AddNode(nodeId).NewDiagram;
            return this;
        }

        [NotNull]
        public DiagramBuilder AddNodes([NotNull] params (string name, double width, double height)[] nodeSpecifications)
        {
            foreach (var nodeSpecification in nodeSpecifications)
                AddNode(nodeSpecification.name, nodeSpecification.width, nodeSpecification.height);

            return this;
        }

        [NotNull]
        public DiagramBuilder AddChildNodes(
            [NotNull] string parentNodeName,
            [NotNull] params (string name, double width, double height)[] childNodeSpecifications)
        {
            foreach (var childNodeSpecification in childNodeSpecifications)
                AddNode(childNodeSpecification.name, childNodeSpecification.width, childNodeSpecification.height, parentNodeName);

            return this;
        }

        [NotNull]
        public DiagramBuilder UpdateNodeTopLeft([NotNull] string nodeName, double nodeTop, double nodeLeft)
        {
            var modelNode = GetModelNodeByName(nodeName);
            var newTopLeft = new Point2D(nodeTop, nodeLeft);
            Diagram = Diagram.UpdateNodeTopLeft(modelNode.Id, newTopLeft).NewDiagram;

            return this;
        }

        [NotNull]
        public DiagramBuilder AddConnector([NotNull] params string[] connectorNames)
        {
            foreach (var connectorName in connectorNames)
            {
                var nodeNames = connectorName.Split(new[] { "->" }, StringSplitOptions.None);
                var sourceNodeId = GetModelNodeByName(nodeNames[0]).Id;
                var targetNodeId = GetModelNodeByName(nodeNames[1]).Id;
                var relationship = GetRelationshipByNodeIds(sourceNodeId, targetNodeId);
                Diagram = Diagram.AddConnector(relationship.Id).NewDiagram;
            }

            return this;
        }

        [NotNull]
        public DiagramBuilder AddRelationship(ModelRelationshipId relationshipId)
        {
            Diagram = Diagram.AddConnector(relationshipId).NewDiagram;
            return this;
        }

        [NotNull]
        public DiagramBuilder AddAllRelationships()
        {
            foreach (var modelRelationship in Diagram.Model.Relationships)
                AddRelationship(modelRelationship.Id);

            return this;
        }

        [NotNull]
        public IModelNode GetModelNodeByName(string nodeName) => Diagram.Model.Nodes.Single(i => i.Name == nodeName);

        [NotNull]
        public IDiagramNode GetDiagramNodeByName(string nodeName)
        {
            var modelNodeId = GetModelNodeByName(nodeName).Id;
            return Diagram.Nodes.Single(i => i.Id == modelNodeId);
        }

        private void AddNode(string nodeName, double nodeWidth, double nodeHeight, [CanBeNull] string parentNodeName = null)
        {
            var parentNodeId = parentNodeName == null
                ? (ModelNodeId?)null
                : GetModelNodeByName(parentNodeName).Id;

            var modelNode = GetModelNodeByName(nodeName);
            Diagram = Diagram.AddNode(modelNode.Id, parentNodeId).NewDiagram;

            var size = new Size2D(nodeWidth, nodeHeight);
            Diagram = Diagram.UpdateNodePayloadAreaSize(modelNode.Id, size).NewDiagram;
        }

        [NotNull]
        private IModelRelationship GetRelationshipByNodeIds(ModelNodeId sourceNodeId, ModelNodeId targetNodeId)
            => Diagram.Model.Relationships.Single(i => i.Source == sourceNodeId && i.Target == targetNodeId);

        /// <summary>
        /// A dummy resolver that always returns Dependency connector type.
        /// </summary>
        private sealed class DummyConnectorTypeResolver : IConnectorTypeResolver
        {
            public ConnectorType GetConnectorType(ModelRelationshipStereotype stereotype) => ConnectorTypes.Dependency;
        }
    }
}