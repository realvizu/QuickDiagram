using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.UI;
using Codartis.SoftVis.UI.Wpf.ViewModel;
using Codartis.Util.UI;
using Codartis.Util.UI.Wpf;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.UI
{
    public sealed class RoslynDiagramShapeViewModelFactory : DiagramShapeViewModelFactory, IRoslynDiagramShapeUiFactory
    {
        public bool IsDescriptionVisible { get; set; }

        public RoslynDiagramShapeViewModelFactory(
            [NotNull] IModelEventSource modelEventSource,
            [NotNull] IDiagramEventSource diagramEventSource,
            [NotNull] IRelatedNodeTypeProvider relatedNodeTypeProvider,
            bool isDescriptionVisible)
            : base(modelEventSource, diagramEventSource, relatedNodeTypeProvider)
        {
            IsDescriptionVisible = isDescriptionVisible;
        }

        public override IDiagramNodeUi CreateDiagramNodeUi(
            IDiagramNode diagramNode,
            IFocusTracker<IDiagramShapeUi> focusTracker)
        {
            var payload = diagramNode.ModelNode.Payload;

            if (!(payload is ISymbol symbol))
                return null;

            var headerUi = CreateDiagramNodeHeaderUi(symbol);

            return new RoslynDiagramNodeViewModel(
                ModelEventSource,
                DiagramEventSource,
                diagramNode,
                RelatedNodeTypeProvider,
                (IWpfFocusTracker<IDiagramShapeUi>)focusTracker,
                IsDescriptionVisible,
                symbol,
                headerUi);
        }

        private RoslynDiagramNodeHeaderViewModelBase CreateDiagramNodeHeaderUi(ISymbol symbol)
        {
            switch (symbol)
            {
                case INamedTypeSymbol namedTypeSymbol:
                    return new RoslynTypeDiagramNodeHeaderViewModel(namedTypeSymbol, IsDescriptionVisible);

                default:
                    return new RoslynMemberDiagramNodeHeaderViewModel(symbol);
            }
        }
    }
}