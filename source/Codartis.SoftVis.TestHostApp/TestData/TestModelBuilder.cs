using System;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.TestHostApp.Modeling;
using Codartis.Util;

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

            if (baseName == null)
                return this;

            ModelService.TryGetTestNodeByName(baseName).Match(
                some => AddBase(name, baseName),
                () => throw new Exception($"Base {baseName} wa not found.")
            );

            return this;
        }

        public TestModelBuilder AddInterface(string name, string baseName = null)
        {
            var node = new InterfaceNode(name);
            ModelService.AddNode(node);

            if (baseName == null)
                return this;

            ModelService.TryGetTestNodeByName(baseName).Match(
                some => AddBase(name, baseName),
                () => throw new Exception($"Base {baseName} wa not found.")
            );

            return this;
        }

        public TestModelBuilder AddBase(string name, string baseName)
        {
            AddRelationship(name, baseName, ModelRelationshipStereotypes.Inheritance);
            return this;
        }

        public TestModelBuilder AddImplements(string name, string interfaceName)
        {
            AddRelationship(name, interfaceName, ModelRelationshipStereotypes.Implementation);
            return this;
        }

        public TestModelBuilder AddProperty(string typeName, string propertyName, string propertyTypeName)
        {
            var ownerTypeNode = typeName == null ? null : ModelService.TryGetTestNodeByName(typeName).Value;

            var propertyNode = new PropertyNode(propertyName);
            ModelService.AddNode(propertyNode, ownerTypeNode);

            if (propertyTypeName != null)
                AddAssociation(propertyName, propertyTypeName);

            return this;
        }

        public TestModelBuilder AddChild(string parentName, string childName)
        {
            AddRelationship(parentName, childName, ModelRelationshipStereotype.Containment);
            return this;
        }

        public TestModelBuilder AddAssociation(string sourceName, string targetName)
        {
            AddRelationship(sourceName, targetName, ModelRelationshipStereotypes.Association);
            return this;
        }

        public TestModelBuilder EndGroup()
        {
            ModelService.StartNewGroup();
            return this;
        }

        private void AddRelationship(string sourceName, string targetName, ModelRelationshipStereotype stereotype)
        {
            var parentNode = ModelService.TryGetUnderlyingNodeByName(sourceName).Value;
            var childNode = ModelService.TryGetUnderlyingNodeByName(targetName).Value;
            ModelService.AddRelationship(parentNode.Id, childNode.Id, stereotype);
        }
    }
}