using Codartis.SoftVis.Modeling2;
using Codartis.SoftVis.Modeling2.Implementation;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    internal class TestClass : TestType
    {
        public TestClass(ModelItemId id, string displayName, string fullName, string description, ModelOrigin origin, bool isAbstract)
            : base(id, displayName, fullName, description, origin, isAbstract)
        {
        }

        protected override ImmutableModelNodeBase CreateInstance(ModelItemId id, string displayName, string fullName, string description, ModelOrigin origin)
        {
            return new TestClass(id, displayName, fullName, description, origin, IsAbstract);
        }
    }
}
