using Codartis.SoftVis.Modeling2;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    /// <summary>
    /// A model node created from a Roslyn delegate symbol.
    /// </summary>
    internal class RoslynDelegateNode : RoslynTypeNode
    {
        internal RoslynDelegateNode(ModelItemId id, INamedTypeSymbol namedTypeSymbol)
            : base(id, namedTypeSymbol, NodeStereotype.Delegate)
        {
        }

        public override int LayoutPriority => 0;
    }
}
