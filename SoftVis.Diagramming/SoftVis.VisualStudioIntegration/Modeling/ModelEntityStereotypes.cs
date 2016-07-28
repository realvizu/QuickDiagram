using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling
{
    /// <summary>
    /// Defines the model entity stereotypes used in Roslyn based models.
    /// </summary>
    internal static class ModelEntityStereotypes
    {
        public static readonly ModelEntityStereotype Class = new ModelEntityStereotype("class");
        public static readonly ModelEntityStereotype Interface = new ModelEntityStereotype("interface");
        public static readonly ModelEntityStereotype Struct = new ModelEntityStereotype("struct");
        public static readonly ModelEntityStereotype Enum = new ModelEntityStereotype("enum");
        public static readonly ModelEntityStereotype Delegate = new ModelEntityStereotype("delegate");
    }
}
