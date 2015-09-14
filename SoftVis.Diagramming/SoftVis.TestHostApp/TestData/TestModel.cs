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

        private TestModel AddEntity(string name, ModelEntityType type)
        {
            if (_entities.Any(i => i.Name == name))
                throw new InvalidOperationException($"Entity with name {name} already exists.");

            IModelEntity newEntity;
            switch (type)
            {
                case (ModelEntityType.Class):
                    newEntity = new TestClass(name);
                    break;
                case (ModelEntityType.Interface):
                    newEntity = new TestInterface(name);
                    break;
                default:
                    throw new ArgumentException($"Unexpected entity type: {type}");
            }
            _entities.Add(newEntity);

            return this;
        }

        private TestModel AddRelationship(string sourceName, ModelRelationshipType type, string targetName)
        {
            if (_relationships.Any(i => i.Source.Name == sourceName && i.Type == type && i.Target.Name == targetName))
                throw new InvalidOperationException($"Relationship already exists {sourceName}--{type}-->{targetName}.");

            var sourceEntity = _entities.FirstOrDefault(i => i.Name == sourceName) as TestModelEntity;
            if (sourceEntity == null)
                throw new InvalidOperationException($"Entity with name {sourceName} not found.");

            var targetEntity = _entities.FirstOrDefault(i => i.Name == targetName) as TestModelEntity;
            if (targetEntity == null)
                throw new InvalidOperationException($"Entity with name {targetName} not found.");

            var newRelationship = new TestModelRelationship(sourceEntity, targetEntity, type);
            sourceEntity.AddOutgoingRelationship(newRelationship);
            targetEntity.AddIncomingRelationship(newRelationship);
            _relationships.Add(newRelationship);

            return this;
        }

        private TestModel AddEntityWithOptionalBase(string name, ModelEntityType type, string baseName = null)
        {
            var model = AddEntity(name, type);

            if (baseName != null)
                model = AddRelationship(name, ModelRelationshipType.Generalization, baseName);

            return model;
        }

        private TestModel AddInterface(string name, string baseName = null)
        {
            return AddEntityWithOptionalBase(name, ModelEntityType.Interface, baseName);
        }

        private TestModel AddClass(string name, string baseName = null)
        {
            return AddEntityWithOptionalBase(name, ModelEntityType.Class, baseName);
        }

        public static TestModel Create()
        {
            return new TestModel()

                .AddInterface("BaseInterface")
                .AddInterface("MyInterface1", baseName: "BaseInterface")
                .AddInterface("MyInterface2", baseName: "BaseInterface")
                .AddInterface("MyInterface3", baseName: "BaseInterface")
                .AddInterface("MyInterface3Child1LongName", baseName: "MyInterface3")

                .AddClass("BaseClass")
                .AddClass("MyClass", baseName: "BaseClass")
                .AddClass("Child1", baseName: "MyClass")
                .AddClass("Child2", baseName: "MyClass")
                .AddClass("Child1OfChild1WithLongName", baseName: "Child1")
                ;
        }
    }
}
