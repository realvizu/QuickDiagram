using Codartis.SoftVis.Modeling.Definition;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    /// <summary>
    /// A model node created from a Roslyn enum symbol.
    /// </summary>
    internal sealed class RoslynEnum : RoslynTypeBase
    {
        internal RoslynEnum([NotNull] INamedTypeSymbol namedTypeSymbol)
            : base(namedTypeSymbol)
        {
        }
    }
}
