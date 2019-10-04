using System;
using System.Linq;
using Codartis.SoftVis.Modeling.Definition;
using JetBrains.Annotations;

namespace Codartis.SoftVis.UnitTests.Modeling
{
    /// <summary>
    /// Test helper for creating models in a more readable way.
    /// Ignores events returned by model mutators. Uses names instead of IDs.
    /// </summary>
    public sealed class ModelBuilder
    {
        [NotNull] public IModel Model { get; private set; }

        public ModelBuilder([NotNull] params IModelRuleProvider[] modelRuleProviders)
        {
            Model = SoftVis.Modeling.Implementation.Model.Create(modelRuleProviders);
        }

        public ModelBuilder([NotNull] IModel model)
        {
            Model = model;
        }

        [NotNull]
        public ModelBuilder AddNodes([NotNull] params string[] nodeNames)
        {
            foreach (var nodeName in nodeNames)
                Model = Model.AddNode(nodeName, ModelNodeStereotype.Default).NewModel;

            return this;
        }

        [NotNull]
        public ModelBuilder AddChildNodes(string parentName, params string[] childNames)
        {
            foreach (var childName in childNames)
            {
                AddNodes(childName);
                var parentId = GetNode(parentName).Id;
                var childId = GetNode(childName).Id;
                Model = Model.AddRelationship(parentId, childId, ModelRelationshipStereotype.Containment).NewModel;
            }

            return this;
        }

        [NotNull]
        public ModelBuilder AddRelationships([NotNull] params string[] relationshipNames)
        {
            foreach (var relationshipName in relationshipNames)
            {
                var nodeNames = GetNodeNames(relationshipName);
                var sourceId = GetNode(nodeNames[0]).Id;
                var targetId = GetNode(nodeNames[1]).Id;
                Model = Model.AddRelationship(sourceId, targetId, ModelRelationshipStereotype.Default).NewModel;
            }

            return this;
        }

        [NotNull]
        public IModelNode GetNode([NotNull] string nodeName) => Model.Nodes.Single(i => i.Name == nodeName);

        [NotNull]
        public IModelRelationship GetRelationship([NotNull] string relationshipName)
        {
            var nodeNames = GetNodeNames(relationshipName);
            var sourceId = GetNode(nodeNames[0]).Id;
            var targetId = GetNode(nodeNames[1]).Id;
            return Model.Relationships.Single(i => i.Source == sourceId && i.Target == targetId);
        }

        [NotNull]
        [ItemNotNull]
        private static string[] GetNodeNames([NotNull] string relationshipName)
        {
            return relationshipName.Split(new[] { "->" }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}