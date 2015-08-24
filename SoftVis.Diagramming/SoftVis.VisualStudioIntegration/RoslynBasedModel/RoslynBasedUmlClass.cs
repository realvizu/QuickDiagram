using Codartis.SoftVis.Modeling;
using Microsoft.CodeAnalysis;
using System.Diagnostics;

namespace Codartis.SoftVis.VisualStudioIntegration.RoslynBasedModel
{
    [DebuggerDisplay("{Name}")]
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
