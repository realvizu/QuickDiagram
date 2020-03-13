using Codartis.SoftVis.VisualStudioIntegration.Modeling;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.UI
{
    public sealed class RoslynTypeDiagramNodeHeaderViewModel : RoslynDiagramNodeHeaderViewModelBase
    {
        public RoslynTypeDiagramNodeHeaderViewModel(
            [NotNull] ISymbol symbol,
            [NotNull] IRoslynSymbolTranslator roslynSymbolTranslator,
            bool isDescriptionVisible)
            : base(symbol, roslynSymbolTranslator, isDescriptionVisible)
        {
        }
    }
}