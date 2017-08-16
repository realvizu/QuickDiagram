using Codartis.SoftVis.Modeling2;
using Codartis.SoftVis.Modeling2.Implementation;

namespace Codartis.SoftVis.Diagramming.UnitTests.TestSubjects
{
    internal class TestModelNode : ImmutableModelNodeBase
    {
        public TestModelNode(string name = null)
            : this(ModelItemId.Create(), name, ModelOrigin.SourceCode)
        {
        }

        private TestModelNode(ModelItemId id, string name, ModelOrigin origin)
            : base(id, name, origin)
        {
        }

        public override int LayoutPriority => 0;
    }
}
