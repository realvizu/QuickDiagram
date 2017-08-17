using Codartis.SoftVis.Modeling2;
using Codartis.SoftVis.Modeling2.Implementation;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    internal abstract class TypeNodeBase : ModelNodeBase
    {
        public bool IsAbstract { get; }

        protected TypeNodeBase(ModelItemId id, string name, ModelNodeStereotype stereotype, ModelOrigin origin, bool isAbstract)
            : base(id, name, stereotype, origin)
        {
            IsAbstract = isAbstract;
        }

        public string FullName => $"Full name of {Name}";
    }
}
