using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    internal class InterfaceNode : TypeNode
    {
        public InterfaceNode(ModelItemId id, string name, ModelOrigin origin, bool isAbstract)
            : base(id, name, ModelNodeStereotypes.Interface, origin, isAbstract)
        {
        }

        public override int LayoutPriority => 0;
    }
}
