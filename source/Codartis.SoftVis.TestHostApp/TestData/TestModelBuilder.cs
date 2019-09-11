using Codartis.SoftVis.Modeling.Definition;
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
            return AddClass(name, null, isAbstract);
        }

        public TestModelBuilder AddClass(string name, string baseName = null, bool isAbstract = false)
        {
            var node = new ClassNode(name, isAbstract);
            ModelService.AddNode(node);

            if (baseName != null)
            {
                var baseNode = ModelService.GetTestNodeByName(baseName);
                AddInheritance(node, baseNode);
            }

            return this;
        }

        public TestModelBuilder AddInterface(string name, string baseName = null)
        {
            var node = new InterfaceNode(name);
            ModelService.AddNode(node);

            if (baseName != null)
            {
                var baseNode = ModelService.GetTestNodeByName(baseName);
                AddInheritance(node, baseNode);
            }

            return this;
        }

        public TestModelBuilder AddBase(string name, string baseName)
        {
            var node = ModelService.GetTestNodeByName(name);
            var baseNode = ModelService.GetTestNodeByName(baseName);
            AddInheritance(node, baseNode);
            return this;
        }

        public TestModelBuilder AddImplements(string name, string interfaceName)
        {
            var node = ModelService.GetTestNodeByName(name);
            var interfaceNode = ModelService.GetTestNodeByName(interfaceName);
            AddImplements(node, interfaceNode);
            return this;
        }

        public TestModelBuilder AddProperty(string typeName, string propertyName, string propertyTypeName)
        {
            var ownerTypeNode = ModelService.GetTestNodeByName(typeName);
            var propertyNode = new PropertyNode(propertyName);
            ModelService.AddNode(propertyNode, ownerTypeNode);

            var propertyTypeNode = ModelService.GetTestNodeByName(propertyTypeName);
            AddRelationship(propertyNode, propertyTypeNode, ModelRelationshipStereotypes.Association);

            return this;
        }

        public TestModelBuilder EndGroup()
        {
            ModelService.StartNewGroup();
            return this;
        }

        private void AddInheritance(ITestNode node, ITestNode baseNode)
        {
            AddRelationship(node, baseNode, ModelRelationshipStereotypes.Inheritance);
        }

        private void AddImplements(ITestNode node, ITestNode baseNode)
        {
            AddRelationship(node, baseNode, ModelRelationshipStereotypes.Implementation);
        }

        private void AddRelationship(ITestNode node, ITestNode baseNode, ModelRelationshipStereotype stereotype)
        {
            var underlyingNode = ModelService.GetUnderlyingNode(node);
            var underlyingBaseNode = ModelService.GetUnderlyingNode(baseNode);
            ModelService.AddRelationship(underlyingNode.Id, underlyingBaseNode.Id, stereotype);
        }
    }
}