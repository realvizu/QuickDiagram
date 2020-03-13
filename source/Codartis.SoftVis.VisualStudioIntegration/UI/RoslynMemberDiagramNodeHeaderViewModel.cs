using Codartis.SoftVis.VisualStudioIntegration.Modeling;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.UI
{
    public sealed class RoslynMemberDiagramNodeHeaderViewModel : RoslynDiagramNodeHeaderViewModelBase
    {
        public RoslynMemberDiagramNodeHeaderViewModel(
            [NotNull] ISymbol symbol,
            [NotNull] IRoslynSymbolTranslator roslynSymbolTranslator)
            : base(symbol, roslynSymbolTranslator, isDescriptionVisible: false)
        {
        }
    }
}