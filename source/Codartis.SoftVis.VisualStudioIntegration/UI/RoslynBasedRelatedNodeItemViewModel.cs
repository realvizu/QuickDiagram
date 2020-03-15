using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.UI.Wpf.ViewModel;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.UI
{
    public sealed class RoslynBasedRelatedNodeItemViewModelFactory : IRelatedNodeItemViewModelFactory
    {
        [NotNull] private readonly IRoslynSymbolTranslator _roslynSymbolTranslator;

        public RoslynBasedRelatedNodeItemViewModelFactory([NotNull] IRoslynSymbolTranslator roslynSymbolTranslator)
        {
            _roslynSymbolTranslator = roslynSymbolTranslator;
        }

        public IRelatedNodeItemViewModel Create(IModelNode modelNode)
        {
            var symbol = (ISymbol)modelNode.Payload;

            return new RelatedNodeItemViewModel(
                modelNode.Id,
                _roslynSymbolTranslator.GetName(symbol),
                _roslynSymbolTranslator.GetFullName(symbol),
                _roslynSymbolTranslator.GetStereotype(symbol),
                _roslynSymbolTranslator.GetIsStatic(symbol));
        }
    }
}