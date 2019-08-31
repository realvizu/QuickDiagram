using JetBrains.Annotations;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    internal abstract class TypeNodeBase : TestNodeBase
    {
        protected TypeNodeBase([NotNull] string name)
            : base(name)
        {
        }
    }
}