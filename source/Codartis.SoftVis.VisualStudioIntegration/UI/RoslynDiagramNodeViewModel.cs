using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.UI;
using Codartis.SoftVis.UI.Wpf.ViewModel;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;
using Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation;
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

        // The following properties' value don't ever change so no need to create wrappers that call OnPropertyChanged.
        public ModelOrigin Origin { get; }
        public string FullName { get; }
        public string Description { get; }
        public bool DescriptionExists { get; }
        public bool IsAbstract { get; }

        public RoslynDiagramNodeViewModel(
            [NotNull] IModelEventSource modelEventSource,
            [NotNull] IDiagramEventSource diagramEventSource,
            [NotNull] IDiagramNode diagramNode,
            [NotNull] IRelatedNodeTypeProvider relatedNodeTypeProvider,
            [NotNull] IWpfFocusTracker<IDiagramShapeUi> focusTracker,
            bool isDescriptionVisible,
            [NotNull] ISymbol symbol)
            : base(modelEventSource, diagramEventSource, diagramNode, relatedNodeTypeProvider, focusTracker)
        {
            _isDescriptionVisible = isDescriptionVisible;
            _symbol = symbol;

            Origin = symbol.GetOrigin();
            Name = symbol.GetName();
            FullName = symbol.GetFullName();
            Description = symbol.GetDescription();
            DescriptionExists = !string.IsNullOrWhiteSpace(Description);
            IsAbstract = symbol.IsAbstract;
        }

        public bool IsDescriptionVisible
        {
            get { return _isDescriptionVisible; }
            set
            {
                if (_isDescriptionVisible != value)
                {
                    _isDescriptionVisible = value;
                    OnPropertyChanged();
                }
            }
        }

        protected override object GetHeader() => this;

        protected override DiagramNodeViewModel CreateInstance()
        {
            return new RoslynDiagramNodeViewModel(
                ModelEventSource,
                DiagramEventSource,
                DiagramNode,
                RelatedNodeTypeProvider,
                FocusTracker,
                IsDescriptionVisible,
                _symbol);
        }
    }
}