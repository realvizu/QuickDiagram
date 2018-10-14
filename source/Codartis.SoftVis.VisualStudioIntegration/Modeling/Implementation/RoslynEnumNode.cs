using Codartis.SoftVis.Modeling;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    /// <summary>
    /// A model node created from a Roslyn enum symbol.
    /// </summary>
    internal class RoslynEnumNode : RoslynTypeNode
    {
        internal RoslynEnumNode(ModelNodeId id, INamedTypeSymbol namedTypeSymbol)
            : base(id, namedTypeSymbol, ModelNodeStereotypes.Enum)
        {
        }

        protected override IRoslynModelNode CreateInstance(ModelNodeId id, ISymbol newSymbol)
            => new RoslynEnumNode(id, EnsureNamedTypeSymbol(newSymbol));
    }
}
