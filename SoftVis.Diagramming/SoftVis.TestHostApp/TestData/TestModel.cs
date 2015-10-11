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

        private TestModel()
        {
            _entities = new List<IModelEntity>();
            _relationships = new List<IModelRelationship>();
        }

        public IEnumerable<IModelEntity> Entities => _entities;
        public IEnumerable<IModelRelationship> Relationships => _relationships;

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

            return this;
        }

        private TestModel AddRelationship(string sourceName, ModelRelationshipType type, string targetName)
        {
            if (_relationships.Any(i => i.Source.Name == sourceName && i.Type == type && i.Target.Name == targetName))
                throw new InvalidOperationException($"Relationship already exists {sourceName}--{type}-->{targetName}.");

            var sourceEntity = _entities.FirstOrDefault(i => i.Name == sourceName) as ModelEntity;
            if (sourceEntity == null)
                throw new InvalidOperationException($"Entity with name {sourceName} not found.");

            var targetEntity = _entities.FirstOrDefault(i => i.Name == targetName) as ModelEntity;
            if (targetEntity == null)
                throw new InvalidOperationException($"Entity with name {targetName} not found.");

            var newRelationship = new ModelRelationship(sourceEntity, targetEntity, type);
            sourceEntity.AddOutgoingRelationship(newRelationship);
            targetEntity.AddIncomingRelationship(newRelationship);
            _relationships.Add(newRelationship);

            return this;
        }

        private TestModel AddEntityWithOptionalBase(string name, int size, ModelEntityType type, ModelEntityStereotype stereotype,
            string baseName = null)
        {
            var model = AddEntity(name, type, stereotype, size);

            if (baseName != null)
                model = AddRelationship(name, ModelRelationshipType.Generalization, baseName);

            return model;
        }

        private TestModel AddInterface(string name, int size, string baseName = null)
        {
            return AddEntityWithOptionalBase(name, size, ModelEntityType.Class, TestModelEntityStereotype.Interface, baseName);
        }

        private TestModel AddClass(string name, int size, string baseName = null)
        {
            return AddEntityWithOptionalBase(name, size, ModelEntityType.Class, null, baseName);
        }

        private TestModel AddClassBase(string name, string baseName = null)
        {
            return AddRelationship(name, ModelRelationshipType.Generalization, baseName);
        }

        public static TestModel Create()
        {
            return new TestModel()

                .AddClass("1", 40)
                .AddClass("2", 25)
                .AddClass("3", 60)
                .AddClass("4", 35)
                .AddClass("5", 80)
                .AddClass("6", 45)
                .AddClass("7", 10)
                .AddClass("8", 10)
                .AddClass("9", 20)
                .AddClass("10", 30)
                .AddClass("11", 40)
                .AddClass("12", 50)
                .AddClass("13", 60)

                .AddClassBase("3", "2")
                .AddClassBase("4", "2")
                .AddClassBase("5", "2")
                .AddClassBase("6", "2")
                .AddClassBase("7", "2")
                .AddClassBase("8", "2")
                .AddClassBase("9", "2")
                .AddClassBase("10", "1")

                //.AddInterface("BaseInterface")
                //.AddInterface("MyInterface1", baseName: "BaseInterface")
                //.AddInterface("MyInterface2", baseName: "BaseInterface")
                //.AddInterface("MyInterface3", baseName: "BaseInterface")
                //.AddInterface("MyInterface3Child1LongName", baseName: "MyInterface3")

                //.AddClass("BaseClass")
                //.AddClass("MyClass", baseName: "BaseClass")
                //.AddClass("Child1", baseName: "MyClass")
                //.AddClass("Child2", baseName: "MyClass")
                ////.AddClass("Child3", baseName: "BaseClass")
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
                //.AddClass("Child3", baseName: "MyInterface3")
                //.AddClass("Child3", baseName: "Circle4")
                //.AddClass("MyInterface3", baseName: "Circle2")

                //.AddClass("IntermediateInterface", baseName: "BaseInterface")
                //.AddClass("MyInterface1", baseName: "IntermediateInterface")

                ;
        }
    }
}
