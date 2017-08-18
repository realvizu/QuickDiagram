using System.Linq;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.UI.Wpf.ViewModel;

namespace Codartis.SoftVis.VisualStudioIntegration.UI
{
    /// <summary>
    /// The diagram viewmodel extended with Roslyn-specific behavior.
    /// </summary>
    internal class RoslynDiagramViewModel : DiagramViewModel
    {
        private bool _areDiagramNodeDescriptionsVisible;

        public void ExpandAllNodes() => SetDiagramNodeDescriptionVisibility(true);
        public void CollapseAllNodes() => SetDiagramNodeDescriptionVisibility(false);

        public RoslynDiagramViewModel(IArrangedDiagram diagram, double minZoom, double maxZoom, double initialZoom, bool initialIsDescriptionVisible)
            : base(diagram, new RoslynDiagramShapeViewModelFactory(diagram, initialIsDescriptionVisible), minZoom, maxZoom, initialZoom)
        {
        }

        private void SetDiagramNodeDescriptionVisibility(bool isVisible)
        {
            _areDiagramNodeDescriptionsVisible = isVisible;

            foreach (var diagramNodeViewModel in DiagramNodeViewModels.OfType<TypeDiagramNodeViewModel>())
                diagramNodeViewModel.IsDescriptionVisible = _areDiagramNodeDescriptionsVisible;
        }

    }
}
