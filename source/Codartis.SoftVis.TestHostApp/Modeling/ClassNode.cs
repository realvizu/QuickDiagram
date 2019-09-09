using JetBrains.Annotations;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    internal sealed class ClassNode : TypeNodeBase
    {
        public ClassNode([NotNull] string name, bool isAbstract)
            : base(name)
        {
            IsAbstract = isAbstract;
        }

        public override bool IsAbstract { get; }
        public override string StereotypeName => "class";
    }
}