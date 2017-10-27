using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Modeling.Implementation;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    internal abstract class TypeMemberNode : ModelNode
    {
        protected TypeMemberNode(ModelNodeId id, string name, ModelNodeStereotype stereotype, ModelOrigin origin)
            : base(id, name, stereotype, origin)
        {
        }

        public string FullName => $"Full name of {Name}";
    }
}
