using JetBrains.Annotations;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.UI
{
    public sealed class RoslynMemberDiagramNodeHeaderViewModel : RoslynDiagramNodeHeaderViewModelBase
    {
        public RoslynMemberDiagramNodeHeaderViewModel([NotNull] ISymbol symbol)
            : base(symbol, isDescriptionVisible: false)
        {
        }
    }
}