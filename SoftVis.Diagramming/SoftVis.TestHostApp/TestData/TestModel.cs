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
        private readonly List<IModelItem> _modelItems;

        private TestModel()
        {
            _entities = new List<IModelEntity>();
            _relationships = new List<IModelRelationship>();
            _modelItems = new List<IModelItem>();
        }

        public IEnumerable<IModelEntity> Entities => _entities;
        public IEnumerable<IModelRelationship> Relationships => _relationships;
        public IEnumerable<IModelItem> Items => _modelItems;

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
            _modelItems.Add(newEntity);

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
            _modelItems.Add(newRelationship);

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


                // Single node
                .AddClass("1", 40)

                // Single connector
                .AddClass("3", 30, "1")

                // Siblings added (left, right, middle)
                .AddClass("2", 45, "1")
                .AddClass("5", 25, "1")
                .AddClass("4", 20, "1")

                // Tree moves under parent
                .AddClass("0", 20)
                .AddBase("1", "0")

                // Placing child with no siblings
                .AddClass("10", 40)
                .AddInterface("11", 50)
                .AddInterface("12", 45, "11")
                .AddClass("13", 40, "10")

                // Dummy vertex added
                .AddInterface("14", 55, "12")
                .AddInterface("15")
                .AddBase("14", "15")

                // Dummy vertex removed
                .AddInterface("16")
                .AddBase("15", "16")

                // Redundant edge removed
                .AddInterface("20", 55)
                .AddBase("12", "20")
                .AddBase("20", "11")

                // Path with 2 new dummy vertices
                .AddInterface("17")
                .AddBase("14", "17")

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

                //.AddInterface("BaseInterface")
                //.AddInterface("MyInterface1", baseName: "BaseInterface")
                //.AddInterface("MyInterface2", baseName: "BaseInterface")
                //.AddInterface("MyInterface3", baseName: "BaseInterface")
                //.AddInterface("MyInterface3Child1LongName", baseName: "MyInterface3")

                //.AddClass("BaseClass")
                //.AddClass("MyClass", baseName: "BaseClass")
                //.AddClass("Child1", baseName: "MyClass")
                //.AddClass("Child2", baseName: "MyClass")
                //.AddClass("Child3", baseName: "BaseClass")
                //.AddClass("Child1OfChild1WithLongName", baseName: "Child1")

                //.AddClass("ForeverAlone")
                //.AddClass("ForeverAlone2")

                //// Loop
                //.AddInterface("Loop")
                //.AddInterface("Loop", baseName: "Loop")

                //// Small circle where edge reversing results a multi-edge
                //.AddInterface("SmallCircle1")
                //.AddInterface("SmallCircle2")
                //.AddInterface("SmallCircle1", baseName: "SmallCircle2")
                //.AddInterface("SmallCircle2", baseName: "SmallCircle1")

                //// Large circle
                //.AddInterface("Circle1")
                //.AddInterface("Circle2")
                //.AddInterface("Circle3")
                //.AddInterface("Circle4")
                //.AddInterface("Circle1", baseName: "Circle2")
                //.AddInterface("Circle2", baseName: "Circle3")
                //.AddInterface("Circle3", baseName: "Circle4")
                //.AddInterface("Circle4", baseName: "Circle1")

                // Edge-crossings
                //.AddClass("Child1", baseName: "BaseInterface")
                //.AddClass("Child3", baseName: "BaseInterface")
                //.AddClass("Child1", baseName: "MyInterface3")
                //.AddClass("Child3", baseName: "Circle4")
                //.AddClass("MyInterface3", baseName: "Circle2")

                //.AddInterface("IntermediateInterface", baseName: "BaseInterface")
                //.AddBase("MyInterface1", "IntermediateInterface")

                ;
        }
    }
}
