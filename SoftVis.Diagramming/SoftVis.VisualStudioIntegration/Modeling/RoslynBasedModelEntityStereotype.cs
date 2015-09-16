using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling
{
    /// <summary>
    /// Defines the model entity stereotypes used in Roslyn based models.
    /// </summary>
    internal class RoslynBasedModelEntityStereotype : ModelEntityStereotype
    {
        public static readonly ModelEntityStereotype Class = new RoslynBasedModelEntityStereotype("class");
        public static readonly ModelEntityStereotype Interface = new RoslynBasedModelEntityStereotype("interface");
        public static readonly ModelEntityStereotype Struct = new RoslynBasedModelEntityStereotype("struct");
        public static readonly ModelEntityStereotype Enum = new RoslynBasedModelEntityStereotype("enum");
        public static readonly ModelEntityStereotype Delegate = new RoslynBasedModelEntityStereotype("delegate");

        private RoslynBasedModelEntityStereotype(string name)
            :base(name)
        {
        }
    }
}
