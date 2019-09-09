using System;
using System.Collections.Immutable;
using System.Linq;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.Modeling.Implementation;
using JetBrains.Annotations;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    internal sealed class TestModelService : ITestModelService
    {
        [NotNull] private readonly IModelService _modelService;

        public IImmutableList<IImmutableList<IModelNode>> ItemGroups { get; private set; }

        public TestModelService(IModelService modelService)
        {
            _modelService = modelService;
            ItemGroups = ImmutableList<IImmutableList<IModelNode>>.Empty;
            StartNewGroup();
        }

        public void StartNewGroup()
        {
            ItemGroups = ItemGroups.Add(ImmutableList.Create<IModelNode>());
        }

        public event Action<ModelEventBase> ModelChanged
        {
            add => _modelService.ModelChanged += value;
            remove => _modelService.ModelChanged -= value;
        }

        public void AddNode(ITestNode node, ITestNode parentNode = null)
        {
            var stereotype = GetNodeType(node);
            var wrapperNode = new ModelNode(ModelNodeId.Create(), node.Name, stereotype, node);
            var parentWrapperNodeId = parentNode == null ? null : (ModelNodeId?)GetWrapperNode(parentNode).Id;

            _modelService.AddNode(wrapperNode, parentWrapperNodeId);
            AddItemToCurrentGroup(wrapperNode);
        }

        public void RemoveNode(ITestNode node)
        {
            _modelService.RemoveNode(GetWrapperNode(node).Id);
        }

        public void AddRelationship(IModelRelationship relationship)
        {
            _modelService.AddRelationship(relationship);
        }

        public void RemoveRelationship(ModelRelationshipId relationshipId)
        {
            _modelService.RemoveRelationship(relationshipId);
        }

        public void ClearModel()
        {
            _modelService.ClearModel();
        }

        public IModelNode GetUnderlyingNode(ITestNode node) => GetWrapperNode(node);

        public ITestNode GetTestNodeByName(string name)
        {
            return (ITestNode)_modelService.LatestModel.Nodes.Single(i => i.Name.Equals(name)).Payload;
        }

        private void AddItemToCurrentGroup(IModelNode modelItem)
        {
            var lastGroup = ItemGroups.Last();
            ItemGroups = ItemGroups.Replace(lastGroup, lastGroup.Add(modelItem));
        }

        private static ModelNodeStereotype GetNodeType(ITestNode node)
        {
            switch (node)
            {
                case ClassNode _:
                    return ModelNodeStereotypes.Class;
                case InterfaceNode _:
                    return ModelNodeStereotypes.Interface;
                case PropertyNode _:
                    return ModelNodeStereotypes.Property;
                default:
                    throw new Exception($"Unexpected node type {node?.GetType().Name}");
            }
        }

        [NotNull]
        private IModelNode GetWrapperNode([NotNull] ITestNode testNode)
        {
            return _modelService.LatestModel.Nodes.Single(i => i.Payload.Equals(testNode));
        }
    }
}