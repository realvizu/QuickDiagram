using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    internal class InterfaceNode : TypeNodeBase
    {
        public InterfaceNode(ModelItemId id, string name, ModelOrigin origin, bool isAbstract)
            : base(id, name, TestModelNodeStereotype.Interface, origin, isAbstract)
        {
        }

        public override int LayoutPriority => 0;
    }
}
