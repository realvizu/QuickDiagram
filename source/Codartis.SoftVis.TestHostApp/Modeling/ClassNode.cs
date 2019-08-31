using JetBrains.Annotations;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    internal sealed class ClassNode : TypeNodeBase
    {
        public bool IsAbstract { get; }

        public ClassNode([NotNull] string name, bool isAbstract)
            : base(name)
        {
            IsAbstract = isAbstract;
        }
    }
}