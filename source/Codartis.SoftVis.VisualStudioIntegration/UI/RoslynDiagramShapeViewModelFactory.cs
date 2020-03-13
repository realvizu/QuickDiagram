using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.UI;
using Codartis.SoftVis.UI.Wpf.ViewModel;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.UI
{
    public sealed class RoslynDiagramShapeViewModelFactory : DiagramShapeViewModelFactory, IRoslynDiagramShapeUiFactory
    {
        [NotNull] private readonly IRoslynSymbolTranslator _roslynSymbolTranslator;

        public bool IsDescriptionVisible { get; set; }

        public RoslynDiagramShapeViewModelFactory(
            [NotNull] IModelEventSource modelEventSource,
            [NotNull] IDiagramEventSource diagramEventSource,
            [NotNull] IRoslynSymbolTranslator roslynSymbolTranslator,
            [NotNull] IRelatedNodeTypeProvider relatedNodeTypeProvider,
            bool isDescriptionVisible)
            : base(modelEventSource, diagramEventSource, relatedNodeTypeProvider)
        {
            _roslynSymbolTranslator = roslynSymbolTranslator;
            IsDescriptionVisible = isDescriptionVisible;
        }

        public override IDiagramNodeUi CreateDiagramNodeUi(IDiagramNode diagramNode)
        {
            var symbol = (ISymbol)diagramNode.ModelNode.Payload;

            var headerUi = CreateDiagramNodeHeaderUi(symbol);

            return new RoslynDiagramNodeViewModel(
                ModelEventSource,
                DiagramEventSource,
                diagramNode,
                IsDescriptionVisible,
                headerUi,
                CreateRelatedNodeCueViewModels(diagramNode));
        }

        [NotNull]
        private RoslynDiagramNodeHeaderViewModelBase CreateDiagramNodeHeaderUi([NotNull] ISymbol symbol)
        {
            RoslynDiagramNodeHeaderViewModelBase result = symbol switch
            {
                INamedTypeSymbol _ => new RoslynTypeDiagramNodeHeaderViewModel(symbol, _roslynSymbolTranslator, IsDescriptionVisible),
                _ => new RoslynMemberDiagramNodeHeaderViewModel(symbol, _roslynSymbolTranslator)
            };
            return result;
        }
    }
}