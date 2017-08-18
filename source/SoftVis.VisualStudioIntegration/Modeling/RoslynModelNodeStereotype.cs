using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling
{
    /// <summary>
    /// Model node stereotypes extended for Roslyn based models.
    /// </summary>
    public class RoslynModelNodeStereotype : ModelNodeStereotype
    {
        public static readonly ModelNodeStereotype Interface = new RoslynModelNodeStereotype(nameof(Interface));
        public static readonly ModelNodeStereotype Struct = new RoslynModelNodeStereotype(nameof(Struct));
        public static readonly ModelNodeStereotype Enum = new RoslynModelNodeStereotype(nameof(Enum));
        public static readonly ModelNodeStereotype Delegate = new RoslynModelNodeStereotype(nameof(Delegate));

        private RoslynModelNodeStereotype(string name) : base(name)
        {
        }
    }
}
