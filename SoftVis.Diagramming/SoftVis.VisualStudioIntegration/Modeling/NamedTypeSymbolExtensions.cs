using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling
{
    public static class NamedTypeSymbolExtensions
    {
        public static string GetFullyQualifiedName(this INamedTypeSymbol namedTypeSymbol)
        {
            return namedTypeSymbol?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        }

        public static string GetMinimallyQualifiedName(this INamedTypeSymbol namedTypeSymbol)
        {
            return namedTypeSymbol?.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
        }

        /// TODO: This is naive. Any way to make it smarter?
        public static bool SymbolEquals(this INamedTypeSymbol namedTypeSymbol1, INamedTypeSymbol namedTypeSymbol2)
        {
            return namedTypeSymbol1?.GetFullyQualifiedName() == namedTypeSymbol2?.GetFullyQualifiedName();
        }
    }
}
