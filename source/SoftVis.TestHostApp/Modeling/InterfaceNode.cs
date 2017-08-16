using Codartis.SoftVis.Modeling2;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    internal class InterfaceNode : TypeNodeBase
    {
        public InterfaceNode(ModelItemId id, string name, ModelOrigin origin, bool isAbstract)
            : base(id, name, origin, isAbstract)
        {
        }

        public override int LayoutPriority => 0;
    }
}
