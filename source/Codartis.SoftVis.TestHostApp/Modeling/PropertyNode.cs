using JetBrains.Annotations;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    internal sealed class PropertyNode : MemberNodeBase
    {
        public bool HasGetter { get; }
        public bool HasSetter { get; }

        public PropertyNode([NotNull] string name, bool hasGetter = true, bool hasSetter = true)
            : base(name)
        {
            HasGetter = hasGetter;
            HasSetter = hasSetter;
        }
    }
}