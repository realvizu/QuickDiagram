using Codartis.SoftVis.Modeling.Definition;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    internal sealed class PropertyNode :  TypeMemberNode 
    {
        public bool HasGetter { get; }
        public bool HasSetter { get; }

        public PropertyNode(ModelNodeId id, string name, ModelOrigin origin = ModelOrigin.SourceCode,
            bool hasGetter = true, bool hasSetter = true)
            : base(id, name, ModelNodeStereotypes.Property, origin)
        {
            HasGetter = hasGetter;
            HasSetter = hasSetter;
        }
    }
}
