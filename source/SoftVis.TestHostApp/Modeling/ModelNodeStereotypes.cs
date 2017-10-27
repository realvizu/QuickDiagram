using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    public static class ModelNodeStereotypes 
    {
        public static readonly ModelNodeStereotype Class = new ModelNodeStereotype(nameof(Class));
        public static readonly ModelNodeStereotype Interface = new ModelNodeStereotype(nameof(Interface));
        public static readonly ModelNodeStereotype Property = new ModelNodeStereotype(nameof(Property));

    }
}
