using System;
using System.Linq;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.Modeling.Implementation;
using JetBrains.Annotations;

namespace Codartis.SoftVis.UnitTests.Modeling
{
    public sealed class ModelBuilder
    {
        [NotNull] public IModel Model { get; private set; }

        public ModelBuilder()
        {
            Model = SoftVis.Modeling.Implementation.Model.Empty;
        }

        [NotNull]
        public ModelBuilder AddNodes([NotNull] params string[] nodeNames)
        {
            foreach (var nodeName in nodeNames)
            {
                var modelNode = CreateModelNode(nodeName);
                Model = Model.AddNode(modelNode);
            }

            return this;
        }

        [NotNull]
        public ModelBuilder AddChildNodes(string parentName, params string[] childNames)
        {
            foreach (var childName in childNames)
            {
                AddNodes(childName);
                var relationship = CreateModelRelationship(parentName, childName, ModelRelationshipStereotype.Containment);
                Model = Model.AddRelationship(relationship);
            }

            return this;
        }

        [NotNull]
        public ModelBuilder AddRelationships([NotNull] params string[] relationshipNames)
        {
            foreach (var relationshipName in relationshipNames)
            {
                var nodeNames = relationshipName.Split(new[] { "->" }, StringSplitOptions.None);
                var relationship = CreateModelRelationship(nodeNames[0], nodeNames[1], ModelRelationshipStereotype.Default);
                Model = Model.AddRelationship(relationship);
            }

            return this;
        }

        public ModelNodeId GetNodeIdByName(string nodeName) => Model.Nodes.Single(i => i.Name == nodeName).Id;

        [NotNull]
        private static IModelNode CreateModelNode([NotNull] string nodeName)
        {
            return new ModelNode(ModelNodeId.Create(), nodeName, ModelNodeStereotype.Default);
        }

        [NotNull]
        private IModelRelationship CreateModelRelationship(
            [NotNull] string sourceNodeName,
            [NotNull] string targetNodeName,
            ModelRelationshipStereotype? stereotype = null)
        {
            var sourceNodeId = GetNodeIdByName(sourceNodeName);
            var targetNodeId = GetNodeIdByName(targetNodeName);
            return new ModelRelationship(ModelRelationshipId.Create(), sourceNodeId, targetNodeId, stereotype ?? ModelRelationshipStereotype.Default);
        }
    }
}