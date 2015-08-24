using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.RoslynBasedModel
{
    public static class INamedTypeSymbolExtensions
    {
        public static string GetFullyQualifiedName(this INamedTypeSymbol namedTypeSymbol)
        {
            return namedTypeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        }
    }
}
