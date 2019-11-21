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
            [NotNull] IModelService modelService,
            [NotNull] IDiagramService diagramService,
            [NotNull] IRelatedNodeTypeProvider relatedNodeTypeProvider,
            [NotNull] IWpfFocusTracker<IDiagramShapeUi> focusTracker,
            [NotNull] IDiagramNode diagramNode,
            bool isDescriptionVisible)
            : base(modelService, diagramService, relatedNodeTypeProvider, focusTracker, diagramNode)
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
                ModelService,
                DiagramService,
                RelatedNodeTypeProvider,
                FocusTracker,
                DiagramNode,
                IsDescriptionVisible);

            SetPropertiesForImageExport(clone);

            return clone;
        }
    }
}