using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.Modeling.Implementation;

namespace Codartis.SoftVis.UnitTests.Modeling
{
    internal class TestModelNode : ModelNode
    {
        public TestModelNode(string name = "dummy")
            : this(ModelNodeId.Create(), name, ModelOrigin.SourceCode)
        {
        }

        private TestModelNode(ModelNodeId id, string name, ModelOrigin origin)
            : base(id, name, TestModelNodeStereotypes.Class, origin)
        {
        }
    }
}
