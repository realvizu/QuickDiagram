using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Modeling.Implementation;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    internal abstract class TypeNode : ModelNode
    {
        public bool IsAbstract { get; }

        protected TypeNode(ModelItemId id, string name, ModelNodeStereotype stereotype, ModelOrigin origin, bool isAbstract)
            : base(id, name, stereotype, origin)
        {
            IsAbstract = isAbstract;
        }

        public string FullName => $"Full name of {Name}";
    }
}
