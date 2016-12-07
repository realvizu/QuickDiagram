using System.Linq;
using Codartis.SoftVis.Modeling;
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
    }
}
