using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.TestHostApp.TestData
{
    public class TestModel
    {
        public static UmlModel Create()
        {
            var baseInterface = new UmlInterface
            {
                Name = "BaseInterface",
            };
            var interface1 = new UmlInterface
            {
                Name = "MyInterface1",
                BaseType = baseInterface
            };
            var interface2 = new UmlInterface
            {
                Name = "MyInterface2",
                BaseType = baseInterface
            };
            var interface3 = new UmlInterface
            {
                Name = "MyInterface3",
                BaseType = baseInterface
            };

            var baseClass = new UmlClass
            {
                Name = "BaseClass",
            };
            var myClass = new UmlClass
            {
                Name = "MyClass",
                BaseType = baseClass,
            };
            var child1 = new UmlClass
            {
                Name = "Child1",
                BaseType = myClass,
            };
            var child2 = new UmlClass
            {
                Name = "Child2",
                BaseType = myClass,
            };

            return new UmlModel {baseInterface, interface1, interface2, interface3, baseClass, myClass, child1, child2};
        }
    }
}
