using JetBrains.Annotations;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.UI
{
    public sealed class RoslynTypeViewModel : RoslynSymbolViewModelBase
    {
        public RoslynTypeViewModel([NotNull] INamedTypeSymbol namedTypeSymbol)
            : base(namedTypeSymbol)
        {
        }
    }
}