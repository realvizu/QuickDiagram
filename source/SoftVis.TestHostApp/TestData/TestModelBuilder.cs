using System.Linq;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.TestHostApp.Modeling;

namespace Codartis.SoftVis.TestHostApp.TestData
{
    internal class TestModelBuilder
    {
        public TestModelStore ModelStore { get; }

        public TestModelBuilder(TestModelStore modelStore)
        {
            ModelStore = modelStore;
        }

        public TestModelBuilder AddClass(string name, bool isAbstract)
        {
            return AddClass(name, null, ModelOrigin.SourceCode, isAbstract);
        }

        public TestModelBuilder AddClass(string name, string baseName = null, ModelOrigin origin = ModelOrigin.SourceCode, bool isAbstract = false)
        {
            var node = new ClassNode(ModelNodeId.Create(), name, origin, isAbstract);
            ModelStore.AddNode(node);
            ModelStore.AddItemToCurrentGroup(node);

            if (baseName != null)
            {
                var baseNode = GetNodeByName(baseName);
                AddInheritance(node, baseNode);
            }

            return this;
        }

        public TestModelBuilder AddInterface(string name, string baseName = null, ModelOrigin origin = ModelOrigin.SourceCode)
        {
            var node = new InterfaceNode(ModelNodeId.Create(), name, origin, false);
            ModelStore.AddNode(node);
            ModelStore.AddItemToCurrentGroup(node);

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
            ModelStore.StartNewGroup();
            return this;
        }

        private IModelNode GetNodeByName(string name)
        {
            return ModelStore.CurrentModel.Nodes.Single(i => i.Name == name);
        }

        private void AddInheritance(IModelNode node, IModelNode baseNode)
        {
            var relationship = new InheritanceRelationship(ModelRelationshipId.Create(), node, baseNode);
            ModelStore.AddRelationship(relationship);
        }

        private void AddImplements(IModelNode node, IModelNode baseNode)
        {
            var relationship = new ImplementationRelationship(ModelRelationshipId.Create(), node, baseNode);
            ModelStore.AddRelationship(relationship);
        }
    }
}
