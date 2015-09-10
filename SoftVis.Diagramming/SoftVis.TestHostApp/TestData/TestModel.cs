using System.Collections.Generic;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.TestHostApp.TestData
{
    public class TestModel : IModel
    {
        private readonly List<IModelEntity> _entities;

        private TestModel(params IModelEntity[]  entities)
        {
            _entities = new List<IModelEntity>(entities);
        }

        public IEnumerable<IModelEntity> Entities => _entities;

        public static TestModel Create()
        {
            var baseInterface = new TestInterface("BaseInterface");

            var interface1 = new TestInterface("MyInterface1",  baseInterface);
            var interface2 = new TestInterface("MyInterface2", baseInterface);
            var interface3 = new TestInterface("MyInterface3", baseInterface);
            var interface3Child1 = new TestInterface("MyInterface3Child1LongName", interface3);

            var baseClass = new TestClass("BaseClass");
            var myClass = new TestClass("MyClass", baseClass);
            var child1 = new TestClass("Child1", myClass);
            var child2 = new TestClass("Child2", myClass);
            var child1Of1 = new TestClass("Child1OfChild1WithLongName", child1);

            return new TestModel(
                baseInterface, interface1, interface2, interface3, interface3Child1,
                baseClass, myClass, child1, child2, child1Of1);
        }
    }
}
