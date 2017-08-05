using System.Collections.Immutable;
using Codartis.SoftVis.Modeling2;
using Codartis.SoftVis.Modeling2.Implementation;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    internal abstract class TestType : ImmutableModelNode
    {
        protected TestType(string displayName, string fullName, string description, ModelOrigin origin)
            : base(ImmutableList<ImmutableModelNode>.Empty, displayName, fullName, description, origin)
        {
        }
    }
}
