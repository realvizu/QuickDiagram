using JetBrains.Annotations;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.UI
{
    public sealed class RoslynTypeDiagramNodeHeaderViewModel : RoslynDiagramNodeHeaderViewModelBase
    {
        public RoslynTypeDiagramNodeHeaderViewModel([NotNull] ISymbol symbol, bool isDescriptionVisible)
            : base(symbol, isDescriptionVisible)
        {
        }
    }
}
