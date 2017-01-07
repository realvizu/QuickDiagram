using System.Linq;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Util.Roslyn;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling
{
    public static class NamedTypeSymbolExtensions
    {
        public static ModelOrigin GetOrigin(this INamedTypeSymbol namedTypeSymbol)
        {
            if (namedTypeSymbol == null)
                return ModelOrigin.Unknown;

            return namedTypeSymbol.Locations.Any(i => i.IsInSource)
                ? ModelOrigin.SourceCode
                : ModelOrigin.Metadata;
        }

        public static string GetName(this INamedTypeSymbol namedTypeSymbol) => namedTypeSymbol.GetMinimallyQualifiedName();
        public static string GetFullName(this INamedTypeSymbol namedTypeSymbol) => namedTypeSymbol.GetNamespaceQualifiedName();
        public static string GetDescription(this INamedTypeSymbol namedTypeSymbol) => namedTypeSymbol.GetCommentSummary()?.Trim();
    }
}
