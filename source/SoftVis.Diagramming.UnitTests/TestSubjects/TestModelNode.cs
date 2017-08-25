using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Modeling.Implementation;

namespace Codartis.SoftVis.Diagramming.UnitTests.TestSubjects
{
    internal class TestModelNode : ModelNode
    {
        public TestModelNode(string name = "dummy")
            : this(ModelItemId.Create(), name, ModelOrigin.SourceCode)
        {
        }

        private TestModelNode(ModelItemId id, string name, ModelOrigin origin)
            : base(id, name, ModelNodeStereotype.Class, origin)
        {
        }

        public override int LayoutPriority => 0;
    }
}
