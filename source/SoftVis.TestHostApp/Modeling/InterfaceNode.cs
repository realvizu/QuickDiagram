using Codartis.SoftVis.Modeling2;
using Codartis.SoftVis.Modeling2.Implementation;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    internal class InterfaceNode : TypeNodeBase
    {
        public InterfaceNode(ModelItemId id, string name, ModelOrigin origin, bool isAbstract)
            : base(id, name, origin, isAbstract)
        {
        }

        public override int LayoutPriority => 0;

        protected override ImmutableModelNodeBase CreateInstance(ModelItemId id, string name, ModelOrigin origin) => 
            new InterfaceNode(id, name, origin, IsAbstract);
    }
}
