using System.Collections.Immutable;
using Codartis.SoftVis.Modeling2;
using Codartis.SoftVis.Modeling2.Implementation;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    internal class TestClass : TestType
    {
        public TestClass(ModelItemId id, string displayName, string fullName, string description, 
            ModelOrigin origin, ImmutableList<ImmutableModelNode> childNodes, bool isAbstract) 
            : base(id, displayName, fullName, description, origin, childNodes, isAbstract)
        {
        }

        protected override ImmutableModelNode CreateInstance(ModelItemId id, 
            string displayName, string fullName, string description, 
            ModelOrigin origin, ImmutableList<ImmutableModelNode> childNodes)
        {
            return new TestClass(id, displayName, fullName, description, origin, childNodes, IsAbstract);
        }
    }
}
