using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling
{
    /// <summary>
    /// Defines the model entity stereotypes used in Roslyn based models.
    /// </summary>
    internal class RoslynBasedModelEntityStereotype : ModelEntityStereotype
    {
        public static readonly RoslynBasedModelEntityStereotype Class = new RoslynBasedModelEntityStereotype("class");
        public static readonly RoslynBasedModelEntityStereotype Interface = new RoslynBasedModelEntityStereotype("interface");
        public static readonly RoslynBasedModelEntityStereotype Struct = new RoslynBasedModelEntityStereotype("struct");
        public static readonly RoslynBasedModelEntityStereotype Enum = new RoslynBasedModelEntityStereotype("enum");
        public static readonly RoslynBasedModelEntityStereotype Delegate = new RoslynBasedModelEntityStereotype("delegate");

        private RoslynBasedModelEntityStereotype(string name)
            :base(name)
        {
        }
    }
}
