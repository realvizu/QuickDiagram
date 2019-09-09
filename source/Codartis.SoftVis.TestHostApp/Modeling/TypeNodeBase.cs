using JetBrains.Annotations;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    internal abstract class TypeNodeBase : TestNodeBase
    {
        protected TypeNodeBase([NotNull] string name)
            : base(name)
        {
        }

        public string FullName => $"Full name of {Name}";
        public abstract string StereotypeName { get; }
    }
}