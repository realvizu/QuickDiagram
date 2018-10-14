using Codartis.SoftVis.Modeling;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    /// <summary>
    /// A model node created from a Roslyn delegate symbol.
    /// </summary>
    internal class RoslynDelegateNode : RoslynTypeNode
    {
        internal RoslynDelegateNode(ModelNodeId id, INamedTypeSymbol namedTypeSymbol)
            : base(id, namedTypeSymbol, ModelNodeStereotypes.Delegate)
        {
        }

        protected override IRoslynModelNode CreateInstance(ModelNodeId id, ISymbol newSymbol)
            => new RoslynDelegateNode(id, EnsureNamedTypeSymbol(newSymbol));
    }
}
