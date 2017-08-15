using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    /// <summary>
    /// A model node created from a Roslyn delegate symbol.
    /// </summary>
    internal class RoslynDelegate : RoslynType
    {
        internal RoslynDelegate(INamedTypeSymbol namedTypeSymbol)
            : base(namedTypeSymbol, TypeKind.Delegate)
        {
        }

        public override int Priority => 0;
    }
}
