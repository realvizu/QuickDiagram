using Codartis.SoftVis.Modeling.Definition;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    internal class InterfaceNode : TypeNode
    {
        public InterfaceNode(ModelNodeId id, string name, ModelOrigin origin, bool isAbstract)
            : base(id, name, ModelNodeStereotypes.Interface, origin, isAbstract)
        {
        }
    }
}
