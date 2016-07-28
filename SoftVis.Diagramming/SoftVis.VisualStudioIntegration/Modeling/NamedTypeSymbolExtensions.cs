using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling
{
    public static class NamedTypeSymbolExtensions
    {
        public static string GetFullyQualifiedName(this INamedTypeSymbol namedTypeSymbol)
        {
            return namedTypeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        }

        public static string GetMinimallyQualifiedName(this INamedTypeSymbol namedTypeSymbol)
        {
            return namedTypeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
        }
    }
}
