using System;
using System.Linq;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Diagramming.Implementation;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling.Definition;
using JetBrains.Annotations;

namespace Codartis.SoftVis.UnitTests.Diagramming
{
    public sealed class DiagramBuilder
    {
        [NotNull] public IDiagram Diagram { get; private set; }

        public DiagramBuilder([NotNull] IModel model)
        {
            Diagram = SoftVis.Diagramming.Implementation.Diagram.Create(model);
        }

        [NotNull]
        public DiagramBuilder AddNodes([NotNull] params (string name, double width, double height)[] nodeSpecifications)
        {
            foreach (var nodeSpecification in nodeSpecifications)
            {
                var modelNode = GetModelNodeByName(nodeSpecification.name);
                var size = new Size2D(nodeSpecification.width, nodeSpecification.height);
                var node = new DiagramNode(modelNode).WithPayloadAreaSize(size);

                Diagram = Diagram.AddNode(node);
            }

            return this;
        }

        [NotNull]
        public DiagramBuilder AddNode(IModelNode modelNode)
        {
            Diagram = Diagram.AddNode(new DiagramNode(modelNode));
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
                var connector = CreateConnector(relationship);
                Diagram = Diagram.AddConnector(connector);
            }

            return this;
        }

        [NotNull]
        public DiagramBuilder AddRelationship([NotNull] IModelRelationship modelRelationship)
        {
            Diagram = Diagram.AddConnector(CreateConnector(modelRelationship));
            return this;
        }

        [NotNull]
        public DiagramBuilder AddAllRelationships()
        {
            foreach (var modelRelationship in Diagram.Model.Relationships)
                AddRelationship(modelRelationship);

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

        [NotNull]
        private IModelRelationship GetRelationshipByNodeIds(ModelNodeId sourceNodeId, ModelNodeId targetNodeId)
            => Diagram.Model.Relationships.Single(i => i.Source == sourceNodeId && i.Target == targetNodeId);

        [NotNull]
        private static DiagramConnector CreateConnector([NotNull] IModelRelationship relationship)
            => new DiagramConnector(relationship, ConnectorTypes.Dependency);
    }
}