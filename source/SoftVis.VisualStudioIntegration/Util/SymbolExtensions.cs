using System.Xml;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Util
{
    public static class SymbolExtensions
    {
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
        public static bool SymbolEquals(this ISymbol namedTypeSymbol1, ISymbol namedTypeSymbol2)
        {
            var originalNamedTypeSymbol1 = namedTypeSymbol1?.OriginalDefinition;
            var originalNamedTypeSymbol2 = namedTypeSymbol2?.OriginalDefinition;

            return originalNamedTypeSymbol1?.GetType() == originalNamedTypeSymbol2?.GetType()
                && originalNamedTypeSymbol1?.GetFullyQualifiedName() == originalNamedTypeSymbol2?.GetFullyQualifiedName();
        }

        public static int GetHashCodeForSymbolEquals(this ISymbol namedTypeSymbol)
            => namedTypeSymbol.GetFullyQualifiedName().GetHashCode();
    }
}