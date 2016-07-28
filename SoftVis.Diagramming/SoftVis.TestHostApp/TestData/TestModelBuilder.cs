using System;
using System.Linq;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Modeling.Implementation;

namespace Codartis.SoftVis.TestHostApp.TestData
{
    internal class TestModelBuilder
    {
        private readonly TestModel _testModel = new TestModel();

        public TestModel Create()
        {
            // Single node
            AddClass("1", 40)
            .EndGroup()

            // Single connector
            .AddClass("3", 30, "1")
            .EndGroup()

            // Siblings added (left, right, middle)
            .AddClass("2", 45, "1")
            .AddClass("5", 25, "1")
            .AddClass("4", 20, "1")
            .EndGroup()

            // Tree moves under parent
            .AddClass("0", 20)
            .AddBase("1", "0")
            .EndGroup()

            // Placing child with no siblings
            .AddClass("10", 40)
            .AddInterface("11", 50)
            .AddInterface("12", 45, "11")
            .AddClass("13", 40, "10")
            .EndGroup()

            // Dummy vertex added
            .AddInterface("14", 55, "12")
            .AddInterface("15")
            .AddBase("14", "15")
            .EndGroup()

            // Dummy vertex removed
            .AddInterface("16")
            .AddBase("15", "16")
            .EndGroup()

            // Redundant edge removed
            .AddInterface("20", 55)
            .AddBase("12", "20")
            .AddBase("20", "11")
            .EndGroup()

            // Path with 2 new dummy vertices
            .AddInterface("17")
            .AddBase("14", "17")
            .EndGroup()

            // Regression test: layer-assignment of dummy vertices should be finished before positioning, 
            // otherwise it causes an infinite cycle in the layout logic
            .AddInterface("A1")
            .AddInterface("A2", 50, "A1")
            .AddInterface("A3", 50, "A2")
            .AddInterface("A4", 50, "A2")
            .AddInterface("A5", 50, "A2")
            .AddInterface("A6", 50, "A5")
            .AddClass("C1")
            .AddClass("C2", 50, "C1")
            .AddClass("C3", 50, "C1")
            .AddImplements("C1", "A3")
            .AddClass("C0")
            .AddBase("C1", "C0")
            .EndGroup()

            // Gap removal
            .AddClass("G1", 40)
            .AddClass("G2", 40, "G1")
            .AddClass("G3", 40, "G1")
            .AddClass("G4", 200, "G3")
            .EndGroup()

            // Overlap removal
            .AddClass("O1", 40)
            .AddClass("O2", 40, "O1")
            .AddInterface("O3")
            .AddInterface("O4", 40, "O3")
            .AddInterface("O5", 40, "O3")
            .AddInterface("O6", 40, "O4")
            .AddInterface("O7", 40, "O5")
            .AddInterface("O8", 40, "O5")
            .AddInterface("O9", 40, "O7")
            .AddInterface("O10", 40, "O8")
            .AddImplements("O2", "O6")
            .AddImplements("O2", "O9")
            .AddImplements("O2", "O10")

            ;

            return _testModel;
        }

        private TestModelBuilder AddEntity(string name, ModelEntityType type, ModelEntityStereotype stereotype, int size)
        {
            if (_testModel.Entities.Any(i => i.Name == name))
                return this;

            ModelEntity newEntity;

            if (type == ModelEntityType.Class && stereotype == ModelEntityStereotype.None)
                newEntity = new TestClass(name, size);
            else if (type == ModelEntityType.Class && stereotype == TestModelEntityStereotypes.Interface)
                newEntity = new TestInterface(name, size);
            else
                throw new ArgumentException($"Unexpected entity type: {type}, stereotype: {stereotype}");

            _testModel.AddEntity(newEntity);
            return this;
        }

        private TestModelBuilder AddRelationship(string sourceName, string targetName,
            ModelRelationshipType type, ModelRelationshipStereotype stereotype)
        {
            if (_testModel.Relationships
                .Any(i => i.Source.Name == sourceName && i.Type == type && i.Stereotype == stereotype && i.Target.Name == targetName))
                throw new InvalidOperationException($"Relationship already exists {sourceName}--{type}({stereotype})-->{targetName}.");

            var sourceEntity = _testModel.Entities.FirstOrDefault(i => i.Name == sourceName) as ModelEntity;
            if (sourceEntity == null)
                throw new InvalidOperationException($"Entity with name {sourceName} not found.");

            var targetEntity = _testModel.Entities.FirstOrDefault(i => i.Name == targetName) as ModelEntity;
            if (targetEntity == null)
                throw new InvalidOperationException($"Entity with name {targetName} not found.");

            var newRelationship = new ModelRelationship(sourceEntity, targetEntity, type, stereotype);
            _testModel.AddRelationship(newRelationship);

            return this;
        }

        private TestModelBuilder EndGroup()
        {
            _testModel.EndGroup();
            return this;
        }

        private TestModelBuilder AddEntityWithOptionalBase(string name, int size, ModelEntityType type, ModelEntityStereotype stereotype,
            string baseName = null)
        {
            var model = AddEntity(name, type, stereotype, size);

            if (baseName != null)
                AddBase(name, baseName);

            return model;
        }

        private TestModelBuilder AddInterface(string name, int size = 100, string baseName = null)
        {
            return AddEntityWithOptionalBase(name, size, ModelEntityType.Class, TestModelEntityStereotypes.Interface, baseName);
        }

        private TestModelBuilder AddClass(string name, int size = 100, string baseName = null)
        {
            return AddEntityWithOptionalBase(name, size, ModelEntityType.Class, ModelEntityStereotype.None, baseName);
        }

        private TestModelBuilder AddBase(string name, string baseName = null)
        {
            return AddRelationship(name, baseName, ModelRelationshipType.Generalization, ModelRelationshipStereotype.None);
        }

        private TestModelBuilder AddImplements(string name, string baseName = null)
        {
            return AddRelationship(name, baseName, ModelRelationshipType.Generalization, TestModelRelationshipStereotypes.Implementation);
        }
    }
}