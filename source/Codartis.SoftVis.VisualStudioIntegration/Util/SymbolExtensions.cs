using System.Xml;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Util
{
    public static class SymbolExtensions
    {
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

        /// TODO: This is naive. Any way to make it smarter?
        public static bool SymbolEquals(this ISymbol symbol1, ISymbol symbol2)
        {
            var originalSymbol1 = symbol1?.OriginalDefinition;
            var originalSymbol2 = symbol2?.OriginalDefinition;

            return originalSymbol1?.GetTypeKind() == originalSymbol2?.GetTypeKind()
                // TODO: add member kind equality check
                && originalSymbol1?.GetFullyQualifiedName() == originalSymbol2?.GetFullyQualifiedName();
        }

        public static int GetHashCodeForSymbolEquals(this ISymbol namedTypeSymbol)
            => namedTypeSymbol.GetFullyQualifiedName().GetHashCode();
    }
}