using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    /// <summary>
    /// A model entity created from a Roslyn delegate symbol.
    /// </summary>
    internal class RoslynBasedDelegate : RoslynBasedModelEntity
    {
        internal RoslynBasedDelegate(INamedTypeSymbol namedTypeSymbol)
            : base(namedTypeSymbol, TypeKind.Delegate)
        {
        }

        public override int Priority => 0;
    }
}
