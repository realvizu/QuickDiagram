using JetBrains.Annotations;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    internal abstract class TypeNodeBase : TestNodeBase
    {
        protected TypeNodeBase([NotNull] string name)
            : base(name)
        {
        }

        public virtual bool IsAbstract => false;
        public string FullName => $"Full name of {Name}";
        public string Stereotype => $"<<{StereotypeName}>>";
        public abstract string StereotypeName { get; }
    }
}