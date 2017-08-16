using System.Linq;
using Codartis.SoftVis.Modeling2;
using Codartis.SoftVis.VisualStudioIntegration.Util;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    public static class SymbolExtensions
    {
        public static ModelOrigin GetOrigin(this ISymbol namedTypeSymbol)
        {
            if (namedTypeSymbol == null)
                return ModelOrigin.Unknown;

            return namedTypeSymbol.Locations.Any(i => i.IsInSource)
                ? ModelOrigin.SourceCode
                : ModelOrigin.Metadata;
        }

        public static string GetName(this ISymbol symbol) => symbol.GetMinimallyQualifiedName();
        public static string GetFullName(this ISymbol symbol) => symbol.GetNamespaceQualifiedName();
        public static string GetDescription(this ISymbol symbol) => symbol.GetCommentSummary()?.Trim();
    }
}
