using Codartis.SoftVis.Modeling;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.RoslynBasedModel
{
    public class RoslynBasedUmlClass : UmlClass
    {
        public RoslynBasedUmlClass(INamedTypeSymbol namedTypeSymbol, UmlClass baseType)
        {
            NativeItem = namedTypeSymbol;
            Name = namedTypeSymbol.Name;
            BaseType = baseType;
        }
    }
}
