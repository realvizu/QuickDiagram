using JetBrains.Annotations;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    internal sealed class InterfaceNode : TypeNodeBase
    {
        public InterfaceNode([NotNull] string name)
            : base(name)
        {
        }

        public override string StereotypeName => "interface";
    }
}