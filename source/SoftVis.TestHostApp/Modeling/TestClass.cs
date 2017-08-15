using Codartis.SoftVis.Modeling2;
using Codartis.SoftVis.Modeling2.Implementation;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    internal class TestClass : TestType
    {
        public TestClass(ModelItemId id, string name, ModelOrigin origin, bool isAbstract)
            : base(id, name, origin, isAbstract)
        {
        }

        protected override ImmutableModelNodeBase CreateInstance(ModelItemId id, string name, ModelOrigin origin)
        {
            return new TestClass(id, name, origin, IsAbstract);
        }
    }
}
