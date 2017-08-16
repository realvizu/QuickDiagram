using Codartis.SoftVis.Modeling2;
using Codartis.SoftVis.Modeling2.Implementation;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    internal abstract class TypeNodeBase : ImmutableModelNodeBase
    {
        public bool IsAbstract { get; }

        protected TypeNodeBase(ModelItemId id, string name, ModelOrigin origin, bool isAbstract)
            : base(id, name, origin)
        {
            IsAbstract = isAbstract;
        }

        public string FullName => $"Full name of {Name}";
    }
}
