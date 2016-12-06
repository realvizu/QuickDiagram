using System.Linq;
using Codartis.SoftVis.Modeling;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling
{
    public static class NamedTypeSymbolExtensions
    {
        public static string GetFullyQualifiedName(this INamedTypeSymbol namedTypeSymbol)
        {
            return namedTypeSymbol?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        }

        public static string GetNamespaceQualifiedName(this INamedTypeSymbol namedTypeSymbol)
        {
            return namedTypeSymbol?.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);
        }

        public static string GetMinimallyQualifiedName(this INamedTypeSymbol namedTypeSymbol)
        {
            return namedTypeSymbol?.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
        }

        public static ModelOrigin GetOrigin(this INamedTypeSymbol namedTypeSymbol)
        {
            if (namedTypeSymbol == null)
                return ModelOrigin.Unknown;

            return namedTypeSymbol.Locations.Any(i => i.IsInSource)
                ? ModelOrigin.SourceCode
                : ModelOrigin.Metadata;
        }

        /// TODO: This is naive. Any way to make it smarter?
        public static bool SymbolEquals(this INamedTypeSymbol namedTypeSymbol1, INamedTypeSymbol namedTypeSymbol2)
        {
            return namedTypeSymbol1?.TypeKind == namedTypeSymbol2.TypeKind
                && namedTypeSymbol1.GetFullyQualifiedName() == namedTypeSymbol2.GetFullyQualifiedName();
        }
    }
}
