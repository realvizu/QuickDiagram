using System.Linq;
using System.Xml;
using Codartis.SoftVis.VisualStudioIntegration.Util;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling
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

        public static TypeKind? GetTypeKind(this ISymbol symbol)
        {
            return (symbol as INamedTypeSymbol)?.TypeKind;
        }

        public static string GetFullyQualifiedName(this ISymbol symbol)
        {
            return symbol?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        }

        public static string GetNamespaceQualifiedName(this ISymbol symbol)
        {
            return symbol?.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);
        }

        public static string GetMinimallyQualifiedName(this ISymbol symbol)
        {
            return symbol?.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
        }

        public static string GetCommentSummary(this ISymbol symbol)
        {
            var xmlCommentAsString = symbol.GetDocumentationCommentXml();

            string summary = null;
            try
            {
                summary = new XmlDocumentation(xmlCommentAsString).Summary;
            }
            catch (XmlException)
            {
            }

            return summary;
        }

        public static string GetName(this ISymbol symbol)
        {
            switch (symbol)
            {
                case IMethodSymbol methodSymbol:
                    var parametersString = string.Join(", ", methodSymbol.Parameters.Select(ToDisplayString));
                    return $"{methodSymbol.Name}({parametersString})";
                default:
                    return symbol.GetMinimallyQualifiedName();
            }
        }

        [NotNull]
        private static string ToDisplayString([NotNull] IParameterSymbol parameterSymbol)
        {
            return $"{parameterSymbol.Type?.Name} {parameterSymbol.Name}";
        }

        public static string GetFullName(this ISymbol symbol) => symbol.GetNamespaceQualifiedName();
        public static string GetDescription(this ISymbol symbol) => symbol.GetCommentSummary()?.Trim();
    }
}