using System.Linq;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.UI.Wpf.ViewModel;

namespace Codartis.SoftVis.VisualStudioIntegration.UI
{
    /// <summary>
    /// The diagram viewmodel extended with Roslyn-specific behavior.
    /// </summary>
    internal class RoslynDiagramViewModel : DiagramViewModel
    {
        private IRoslynDiagramShapeUiFactory RoslynDiagramShapeUiFactory => (IRoslynDiagramShapeUiFactory)DiagramShapeUiFactory;

        public void ExpandAllNodes() => SetDiagramNodeDescriptionVisibility(true);
        public void CollapseAllNodes() => SetDiagramNodeDescriptionVisibility(false);

        public RoslynDiagramViewModel(
            IModelService modelService, 
            IDiagramService diagramService,
            bool initialIsDescriptionVisible,
            double minZoom, 
            double maxZoom, 
            double initialZoom)
            : base(
                  modelService, 
                  diagramService,
                  new RoslynDiagramShapeUiFactory(initialIsDescriptionVisible),
                  minZoom, 
                  maxZoom, 
                  initialZoom)
        {
        }

        private void SetDiagramNodeDescriptionVisibility(bool isVisible)
        {
            RoslynDiagramShapeUiFactory.IsDescriptionVisible = isVisible;

            foreach (var diagramNodeViewModel in DiagramNodeViewModels.OfType<RoslynTypeDiagramNodeViewModel>())
                diagramNodeViewModel.IsDescriptionVisible = isVisible;
        }
    }
}
