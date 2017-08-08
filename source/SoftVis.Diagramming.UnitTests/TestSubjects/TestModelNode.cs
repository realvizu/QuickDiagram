using System.Collections.Immutable;
using Codartis.SoftVis.Modeling2;
using Codartis.SoftVis.Modeling2.Implementation;

namespace Codartis.SoftVis.Diagramming.UnitTests.TestSubjects
{
    internal class TestModelNode : ImmutableModelNodeBase
    {
        public TestModelNode(string name = null)
            : this(ModelItemId.Create(), name, name, name, ModelOrigin.SourceCode, ImmutableList<ImmutableModelNodeBase>.Empty)
        {
        }

        private TestModelNode(ModelItemId id, string displayName, string fullName, string description, ModelOrigin origin, ImmutableList<ImmutableModelNodeBase> childNodes)
            : base(id, displayName, fullName, description, origin, childNodes)
        {
        }

        public override int Priority => 0;

        protected override ImmutableModelNodeBase CreateInstance(ModelItemId id, string displayName, string fullName, string description,
            ModelOrigin origin, ImmutableList<ImmutableModelNodeBase> childNodes)
        {
            return new TestModelNode(id, displayName, fullName, description, origin, childNodes);
        }
    }
}
