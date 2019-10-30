using Codartis.SoftVis.Modeling.Definition;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    /// <summary>
    /// A model node created from a Roslyn delegate symbol.
    /// </summary>
    internal sealed class RoslynDelegate : RoslynTypeBase
    {
        internal RoslynDelegate(INamedTypeSymbol namedTypeSymbol)
            : base(namedTypeSymbol)
        {
        }
    }
}
