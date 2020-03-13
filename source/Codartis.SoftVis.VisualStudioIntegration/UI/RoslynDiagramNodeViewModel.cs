using System;
using System.Collections.Generic;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.UI.Wpf.ViewModel;
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

        public RoslynDiagramNodeViewModel(
            [NotNull] IModelEventSource modelEventSource,
            [NotNull] IDiagramEventSource diagramEventSource,
            [NotNull] IDiagramNode diagramNode,
            bool isDescriptionVisible,
            [NotNull] RoslynDiagramNodeHeaderViewModelBase header,
            [NotNull] [ItemNotNull] List<RelatedNodeCueViewModel> relatedCueViewModels)
            : base(
                modelEventSource,
                diagramEventSource,
                diagramNode,
                header,
                relatedCueViewModels)
        {
            _isDescriptionVisible = isDescriptionVisible;
            Name = diagramNode.Name;
        }

        [NotNull] private RoslynDiagramNodeHeaderViewModelBase TypedHeader => (RoslynDiagramNodeHeaderViewModelBase)Header;

        [NotNull] public Type HeaderType => Header.GetType();

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
            var symbol = (ISymbol)diagramNode.ModelNode.Payload;
            TypedHeader.Update(symbol);
        }

        protected override DiagramNodeViewModel CreateInstance()
        {
            return new RoslynDiagramNodeViewModel(
                ModelEventSource,
                DiagramEventSource,
                DiagramNode,
                _isDescriptionVisible,
                TypedHeader,
                RelatedNodeCueViewModels);
        }
    }
}