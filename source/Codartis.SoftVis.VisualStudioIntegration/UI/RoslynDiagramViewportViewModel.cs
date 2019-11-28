using System.Linq;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.UI;
using Codartis.SoftVis.UI.Wpf.ViewModel;
using Codartis.Util.UI;
using JetBrains.Annotations;

namespace Codartis.SoftVis.VisualStudioIntegration.UI
{
    /// <summary>
    /// The view model of a QuickDiagram tool window's viewport.
    /// </summary>
    public sealed class RoslynDiagramViewportViewModel : DiagramViewportViewModel
    {
        [NotNull] private readonly IRoslynDiagramShapeUiFactory _roslynDiagramShapeUiFactory;

        public RoslynDiagramViewportViewModel(
            [NotNull] IModelEventSource modelEventSource,
            [NotNull] IDiagramEventSource diagramEventSource,
            [NotNull] IDiagramShapeUiFactory diagramShapeUiFactory,
            [NotNull] IDecorationManager<IMiniButton, IDiagramShapeUi> miniButtonManager,
            double minZoom,
            double maxZoom,
            double initialZoom)
            : base(modelEventSource, diagramEventSource, diagramShapeUiFactory, miniButtonManager, minZoom, maxZoom, initialZoom)
        {
            _roslynDiagramShapeUiFactory = (IRoslynDiagramShapeUiFactory)diagramShapeUiFactory;
        }

        public void ExpandAllNodes() => SetDiagramNodeDescriptionVisibility(true);
        public void CollapseAllNodes() => SetDiagramNodeDescriptionVisibility(false);

        private void SetDiagramNodeDescriptionVisibility(bool isVisible)
        {
            _roslynDiagramShapeUiFactory.IsDescriptionVisible = isVisible;

            foreach (var diagramNodeViewModel in DiagramNodeViewModels.OfType<RoslynDiagramNodeViewModel>())
                diagramNodeViewModel.IsDescriptionVisible = isVisible;
        }
    }
}