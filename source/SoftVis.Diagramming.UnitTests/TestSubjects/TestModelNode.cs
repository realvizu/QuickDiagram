using System.Collections.Immutable;
using Codartis.SoftVis.Modeling2;
using Codartis.SoftVis.Modeling2.Implementation;

namespace Codartis.SoftVis.Diagramming.UnitTests.TestSubjects
{
    internal class TestModelNode : ImmutableModelNode
    {
        public TestModelNode(string name = null)
            : this(ModelItemId.Create(), name, name, name, ModelOrigin.SourceCode, ImmutableList<ImmutableModelNode>.Empty)
        {
        }

        private TestModelNode(ModelItemId id, string displayName, string fullName, string description, ModelOrigin origin, ImmutableList<ImmutableModelNode> childNodes)
            : base(id, displayName, fullName, description, origin, childNodes)
        {
        }

        public override int Priority => 0;

        protected override ImmutableModelNode CreateInstance(ModelItemId id, string displayName, string fullName, string description,
            ModelOrigin origin, ImmutableList<ImmutableModelNode> childNodes)
        {
            return new TestModelNode(id, displayName, fullName, description, origin, childNodes);
        }
    }
}
