using System;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.TestHostApp.TestData
{
    public class TestModel : IModel
    {
        private readonly List<IModelEntity> _entities;
        private readonly List<IModelRelationship> _relationships;
        private readonly List<List<IModelItem>> _modelItemGroups;

        private TestModel()
        {
            _entities = new List<IModelEntity>();
            _relationships = new List<IModelRelationship>();
            _modelItemGroups = new List<List<IModelItem>>();
            EndGroup();
        }

        public IEnumerable<IModelEntity> Entities => _entities;
        public IEnumerable<IModelRelationship> Relationships => _relationships;
        public IEnumerable<List<IModelItem>> ItemGroups => _modelItemGroups;

        private TestModel AddEntity(string name, ModelEntityType type, ModelEntityStereotype stereotype, int size)
        {
            if (_entities.Any(i => i.Name == name))
                return this;

            IModelEntity newEntity;

            if (type == ModelEntityType.Class && stereotype == null)
                newEntity = new TestClass(name, size);
            else if (type == ModelEntityType.Class && stereotype == TestModelEntityStereotype.Interface)
                newEntity = new TestInterface(name, size);
            else
                throw new ArgumentException($"Unexpected entity type: {type}, stereotype: {stereotype}");

            _entities.Add(newEntity);
            _modelItemGroups.Last().Add(newEntity);

            return this;
        }

        private TestModel AddRelationship(string sourceName, string targetName,
            ModelRelationshipType type, ModelRelationshipStereotype stereotype = null)
        {
            if (_relationships
                .Any(i => i.Source.Name == sourceName && i.Type == type && i.Stereotype == stereotype && i.Target.Name == targetName))
                throw new InvalidOperationException($"Relationship already exists {sourceName}--{type}({stereotype})-->{targetName}.");

            var sourceEntity = _entities.FirstOrDefault(i => i.Name == sourceName) as ModelEntity;
            if (sourceEntity == null)
                throw new InvalidOperationException($"Entity with name {sourceName} not found.");

            var targetEntity = _entities.FirstOrDefault(i => i.Name == targetName) as ModelEntity;
            if (targetEntity == null)
                throw new InvalidOperationException($"Entity with name {targetName} not found.");

            var newRelationship = new ModelRelationship(sourceEntity, targetEntity, type, stereotype);
            sourceEntity.AddOutgoingRelationship(newRelationship);
            targetEntity.AddIncomingRelationship(newRelationship);
            _relationships.Add(newRelationship);
            _modelItemGroups.Last().Add(newRelationship);

            return this;
        }

        private TestModel EndGroup()
        {
            _modelItemGroups.Add(new List<IModelItem>());
            return this;
        }

        private TestModel AddEntityWithOptionalBase(string name, int size, ModelEntityType type, ModelEntityStereotype stereotype,
            string baseName = null)
        {
            var model = AddEntity(name, type, stereotype, size);

            if (baseName != null)
                AddBase(name, baseName);

            return model;
        }

        private TestModel AddInterface(string name, int size = 100, string baseName = null)
        {
            return AddEntityWithOptionalBase(name, size, ModelEntityType.Class, TestModelEntityStereotype.Interface, baseName);
        }

        private TestModel AddClass(string name, int size = 100, string baseName = null)
        {
            return AddEntityWithOptionalBase(name, size, ModelEntityType.Class, null, baseName);
        }

        private TestModel AddBase(string name, string baseName = null)
        {
            return AddRelationship(name, baseName, ModelRelationshipType.Generalization);
        }

        private TestModel AddImplements(string name, string baseName = null)
        {
            return AddRelationship(name, baseName, ModelRelationshipType.Generalization, TestModelRelationshipStereotype.Implementation);
        }

        public static TestModel Create()
        {
            return new TestModel()

                // Single node
                .AddClass("1", 40)
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
        }
    }
}
