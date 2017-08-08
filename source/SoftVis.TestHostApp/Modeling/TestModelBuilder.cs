using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Codartis.SoftVis.Modeling2;
using Codartis.SoftVis.Modeling2.Implementation;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    internal class TestModelBuilder : ModelBuilderBase
    {
        public List<List<ModelItemId>> ItemGroups { get; }

        public TestModelBuilder()
        {
            ItemGroups = new List<List<ModelItemId>>();
            StartNewGroup();
        }

        public TestModelBuilder AddClass(string name, string baseName = null)
        {
            var node = new TestClass(ModelItemId.Create(),  name, name, name, ModelOrigin.SourceCode, ImmutableList<ImmutableModelNodeBase>.Empty, false);
            AddNode(node);
            AddItemToCurrentGroup(node);

            if (baseName != null)
            {
                var baseNode = GetNodeByName(baseName);
                AddInheritance(node, baseNode);
            }

            return this;
        }

        public TestModelBuilder AddInterface(string name, string baseName = null)
        {
            var node = new TestInterface(ModelItemId.Create(), name, name, name, ModelOrigin.SourceCode, ImmutableList<ImmutableModelNodeBase>.Empty, false);
            AddNode(node);
            AddItemToCurrentGroup(node);

            if (baseName != null)
            {
                var baseNode = GetNodeByName(baseName);
                AddInheritance(node, baseNode);
            }

            return this;
        }

        public TestModelBuilder AddBase(string name, string baseName)
        {
            var node = GetNodeByName(name);
            var baseNode = GetNodeByName(baseName);
            AddInheritance(node, baseNode);
            return this;
        }

        public TestModelBuilder AddImplements(string name, string interfaceName)
        {
            var node = GetNodeByName(name);
            var interfaceNode = GetNodeByName(interfaceName);
            AddImplements(node, interfaceNode);
            return this;
        }

        public TestModelBuilder EndGroup()
        {
            StartNewGroup();
            return this;
        }

        private IModelNode GetNodeByName(string name)
        {
            return CurrentModel.Nodes.Single(i => i.DisplayName == name);
        }

        private void AddInheritance(IModelNode node, IModelNode baseNode)
        {
            var relationship = new TestInheritanceRelationship(ModelItemId.Create(), node, baseNode);
            AddRelationship(relationship);
        }

        private void AddImplements(IModelNode node, IModelNode baseNode)
        {
            var relationship = new TestImplementsRelationship(ModelItemId.Create(), node, baseNode);
            AddRelationship(relationship);
        }

        private void AddItemToCurrentGroup(IModelItem modelItem)
        {
            ItemGroups.Last().Add(modelItem.Id);
        }

        private void StartNewGroup()
        {
            ItemGroups.Add(new List<ModelItemId>());
        }
    }
}
