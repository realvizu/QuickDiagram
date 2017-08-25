using Codartis.SoftVis.Modeling;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    /// <summary>
    /// A model node created from a Roslyn enum symbol.
    /// </summary>
    internal class RoslynEnumNode : RoslynTypeNode
    {
        internal RoslynEnumNode(ModelItemId id, INamedTypeSymbol namedTypeSymbol)
            : base(id, namedTypeSymbol, ModelNodeStereotypes.Enum)
        {
        }

        public override int LayoutPriority => 1;
    }
}
