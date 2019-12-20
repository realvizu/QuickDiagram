using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.UI;
using Codartis.SoftVis.UI.Wpf.ViewModel;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;
using Codartis.Util.UI.Wpf;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.UI
{
    /// <summary>
    /// View model for a diagram node that is associated with a Roslyn symbol.
    /// </summary>
    internal sealed class RoslynDiagramNodeViewModel : DiagramNodeViewModel
    {
        private bool _isDescriptionVisible;

        [NotNull] private readonly ISymbol _symbol;

        public RoslynDiagramNodeViewModel(
            [NotNull] IModelEventSource modelEventSource,
            [NotNull] IDiagramEventSource diagramEventSource,
            [NotNull] IDiagramNode diagramNode,
            [NotNull] IRelatedNodeTypeProvider relatedNodeTypeProvider,
            [NotNull] IWpfFocusTracker<IDiagramShapeUi> focusTracker,
            bool isDescriptionVisible,
            [NotNull] ISymbol symbol,
            RoslynDiagramNodeHeaderViewModelBase header)
            : base(
                modelEventSource,
                diagramEventSource,
                diagramNode,
                relatedNodeTypeProvider,
                focusTracker,
                header)
        {
            _isDescriptionVisible = isDescriptionVisible;
            _symbol = symbol;
            Name = symbol.GetName();
        }

        [NotNull] private RoslynDiagramNodeHeaderViewModelBase TypedHeader => (RoslynDiagramNodeHeaderViewModelBase)Header;

        public bool IsDescriptionVisible
        {
            get { return _isDescriptionVisible; }
            set
            {
                if (_isDescriptionVisible != value)
                {
                    _isDescriptionVisible = value;
                    OnPropertyChanged();

                    TypedHeader.IsDescriptionVisible = value;
                }
            }
        }

        protected override void UpdateHeader(IDiagramNode diagramNode)
        {
            TypedHeader.Update((ISymbol)diagramNode.ModelNode.Payload);
        }

        protected override DiagramNodeViewModel CreateInstance()
        {
            return new RoslynDiagramNodeViewModel(
                ModelEventSource,
                DiagramEventSource,
                DiagramNode,
                RelatedNodeTypeProvider,
                FocusTracker,
                _isDescriptionVisible,
                _symbol,
                TypedHeader);
        }
    }
}