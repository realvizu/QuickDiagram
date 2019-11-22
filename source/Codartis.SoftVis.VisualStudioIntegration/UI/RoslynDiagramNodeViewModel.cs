using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.UI;
using Codartis.SoftVis.UI.Wpf.ViewModel;
using Codartis.Util.UI.Wpf;
using JetBrains.Annotations;

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
            [CanBeNull] IPayloadUi payloadUi,
            [NotNull] IRelatedNodeTypeProvider relatedNodeTypeProvider,
            [NotNull] IWpfFocusTracker<IDiagramShapeUi> focusTracker,
            bool isDescriptionVisible)
            : base(modelEventSource, diagramEventSource, diagramNode, payloadUi, relatedNodeTypeProvider, focusTracker)
        {
            IsDescriptionVisible = isDescriptionVisible;
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

        public override object CloneForImageExport()
        {
            var clone = new RoslynDiagramNodeViewModel(
                ModelEventSource,
                DiagramEventSource,
                DiagramNode,
                PayloadUi,
                RelatedNodeTypeProvider,
                FocusTracker,
                IsDescriptionVisible);

            SetPropertiesForImageExport(clone);

            return clone;
        }
    }
}