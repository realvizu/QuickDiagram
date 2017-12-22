using System;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.UI;
using Codartis.SoftVis.UI.Wpf.ViewModel;
using Codartis.SoftVis.Util.UI.Wpf.ViewModels;
using Codartis.SoftVis.VisualStudioIntegration.Diagramming;

namespace Codartis.SoftVis.VisualStudioIntegration.UI
{
    /// <summary>
    /// Creates view models for roslyn-based diagram shapes.
    /// </summary>
    public class RoslynDiagramShapeUiFactory : DiagramShapeUiFactoryBase, IRoslynDiagramShapeUiFactory
    {
        public bool IsDescriptionVisible { get; set; }

        public RoslynDiagramShapeUiFactory(bool isDescriptionVisible)
        {
            IsDescriptionVisible = isDescriptionVisible;
        }

        public override IDiagramNodeUi CreateDiagramNodeUi(IDiagramService diagramService, IDiagramNode diagramNode, 
            IFocusTracker<IDiagramShapeUi> focusTracker)
        {
            if (diagramNode is RoslynTypeDiagramNode roslynTypeDiagramNode)
                return new RoslynTypeDiagramNodeViewModel(ModelService, diagramService, focusTracker, roslynTypeDiagramNode, IsDescriptionVisible);

            throw new InvalidOperationException($"Unexpected diagram node type {diagramNode?.GetType().Name}");
        }
    }
}