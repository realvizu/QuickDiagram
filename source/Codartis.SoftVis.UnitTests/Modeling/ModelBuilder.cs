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
                var modelNode = new ModelNode(ModelNodeId.Create(), nodeName, ModelNodeStereotype.Default);
                Model = Model.AddNode(modelNode);
            }

            return this;
        }

        public ModelBuilder AddChildNodes(string parentName, params string[] childNames)
        {
            throw new NotImplementedException();
        }


        [NotNull]
        public ModelBuilder AddRelationships([NotNull] params string[] relationshipNames)
        {
            foreach (var relationshipName in relationshipNames)
            {
                var nodeNames = relationshipName.Split(new[] { "->" }, StringSplitOptions.None);
                var sourceNodeId = GetNodeIdByName(nodeNames[0]);
                var targetNodeId = GetNodeIdByName(nodeNames[1]);
                var modelRelationship = new ModelRelationship(ModelRelationshipId.Create(), sourceNodeId, targetNodeId, ModelRelationshipStereotype.Default);
                Model = Model.AddRelationship(modelRelationship);
            }

            return this;
        }

        public ModelNodeId GetNodeIdByName(string nodeName) => Model.Nodes.Single(i => i.Name == nodeName).Id;
    }
}