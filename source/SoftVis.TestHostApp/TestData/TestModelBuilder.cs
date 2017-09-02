using System.Linq;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.TestHostApp.Modeling;

namespace Codartis.SoftVis.TestHostApp.TestData
{
    internal class TestModelBuilder
    {
        public ITestModelService ModelService { get; }

        public TestModelBuilder(ITestModelService modelService)
        {
            ModelService = modelService;
        }

        public TestModelBuilder AddClass(string name, bool isAbstract)
        {
            return AddClass(name, null, ModelOrigin.SourceCode, isAbstract);
        }

        public TestModelBuilder AddClass(string name, string baseName = null, ModelOrigin origin = ModelOrigin.SourceCode, bool isAbstract = false)
        {
            var node = new ClassNode(ModelNodeId.Create(), name, origin, isAbstract);
            ModelService.AddNode(node);
            ModelService.AddItemToCurrentGroup(node);

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
            ModelService.AddNode(node);
            ModelService.AddItemToCurrentGroup(node);

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
            ModelService.StartNewGroup();
            return this;
        }

        private IModelNode GetNodeByName(string name)
        {
            return ModelService.Model.Nodes.Single(i => i.Name == name);
        }

        private void AddInheritance(IModelNode node, IModelNode baseNode)
        {
            var relationship = new InheritanceRelationship(ModelRelationshipId.Create(), node, baseNode);
            ModelService.AddRelationship(relationship);
        }

        private void AddImplements(IModelNode node, IModelNode baseNode)
        {
            var relationship = new ImplementationRelationship(ModelRelationshipId.Create(), node, baseNode);
            ModelService.AddRelationship(relationship);
        }
    }
}
