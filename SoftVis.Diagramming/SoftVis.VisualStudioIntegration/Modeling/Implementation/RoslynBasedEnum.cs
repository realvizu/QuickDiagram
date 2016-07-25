using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    /// <summary>
    /// A model entity created from a Roslyn enum symbol.
    /// </summary>
    internal class RoslynBasedEnum : RoslynBasedModelEntity
    {
        internal RoslynBasedEnum(INamedTypeSymbol namedTypeSymbol)
            : base(namedTypeSymbol, TypeKind.Enum)
        {
        }

        public override int Priority => 1;
    }
}
