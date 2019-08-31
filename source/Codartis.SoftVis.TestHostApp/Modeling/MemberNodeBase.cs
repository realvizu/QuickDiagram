using JetBrains.Annotations;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    internal abstract class MemberNodeBase : TestNodeBase
    {
        protected MemberNodeBase([NotNull] string name)
            : base(name)
        {
        }
    }
}
