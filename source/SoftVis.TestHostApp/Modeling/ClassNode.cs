using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    internal class ClassNode : TypeNodeBase
    {
        public ClassNode(ModelItemId id, string name, ModelOrigin origin, bool isAbstract)
            : base(id, name, ModelNodeStereotype.Class,  origin, isAbstract)
        {
        }

        public override int LayoutPriority => 1;
    }
}
