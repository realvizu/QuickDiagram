using Codartis.SoftVis.Modeling2;
using Codartis.SoftVis.Modeling2.Implementation;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    internal class ClassNode : TypeNodeBase
    {
        public ClassNode(ModelItemId id, string name, ModelOrigin origin, bool isAbstract)
            : base(id, name, origin, isAbstract)
        {
        }

        public override int LayoutPriority => 1;
        
        protected override ImmutableModelNodeBase CreateInstance(ModelItemId id, string name, ModelOrigin origin)
        {
            return new ClassNode(id, name, origin, IsAbstract);
        }
    }
}
