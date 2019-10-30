using Codartis.SoftVis.Modeling.Definition;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling
{
    /// <summary>
    /// Model node stereotypes used in Roslyn based models.
    /// </summary>
    public static class ModelNodeStereotypes
    {
        public static readonly ModelNodeStereotype Class = new ModelNodeStereotype(nameof(Class));
        public static readonly ModelNodeStereotype Interface = new ModelNodeStereotype(nameof(Interface));
        public static readonly ModelNodeStereotype Struct = new ModelNodeStereotype(nameof(Struct));
        public static readonly ModelNodeStereotype Enum = new ModelNodeStereotype(nameof(Enum));
        public static readonly ModelNodeStereotype Delegate = new ModelNodeStereotype(nameof(Delegate));
        public static readonly ModelNodeStereotype Property = new ModelNodeStereotype(nameof(Property));
        public static readonly ModelNodeStereotype Method = new ModelNodeStereotype(nameof(Method));
    }
}
