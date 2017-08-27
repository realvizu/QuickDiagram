using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    internal class ClassNode : TypeNode
    {
        public ClassNode(ModelNodeId id, string name, ModelOrigin origin, bool isAbstract)
            : base(id, name, ModelNodeStereotypes.Class,  origin, isAbstract)
        {
        }
    }
}
