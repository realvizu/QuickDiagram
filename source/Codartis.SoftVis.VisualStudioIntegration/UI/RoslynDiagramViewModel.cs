using System.Linq;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.UI;
using Codartis.SoftVis.UI.Wpf.ViewModel;
using JetBrains.Annotations;

namespace Codartis.SoftVis.VisualStudioIntegration.UI
{
    /// <summary>
    /// The diagram viewmodel extended with Roslyn-specific behavior.
    /// </summary>
    internal sealed class RoslynDiagramViewModel : DiagramViewModel
    {
        public RoslynDiagramViewModel(
            [NotNull] IModelService modelService,
            [NotNull] IDiagramService diagramService,
            [NotNull] IDiagramViewportUi diagramViewportUi)
            : base(modelService, diagramService, diagramViewportUi)
        {
        }

        //[NotNull] private IRoslynDiagramShapeUiFactory RoslynDiagramShapeUiFactory => (IRoslynDiagramShapeUiFactory)DiagramShapeUiFactory;

        public void ExpandAllNodes() => SetDiagramNodeDescriptionVisibility(true);
        public void CollapseAllNodes() => SetDiagramNodeDescriptionVisibility(false);

        private void SetDiagramNodeDescriptionVisibility(bool isVisible)
        {
            //RoslynDiagramShapeUiFactory.IsDescriptionVisible = isVisible;

            foreach (var diagramNodeViewModel in DiagramNodeViewModels.OfType<RoslynDiagramNodeViewModel>())
                diagramNodeViewModel.IsDescriptionVisible = isVisible;
        }
    }
}