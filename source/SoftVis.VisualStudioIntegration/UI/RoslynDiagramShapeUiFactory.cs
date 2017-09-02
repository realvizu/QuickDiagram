using System;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.UI.Wpf.ViewModel;
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

        public override DiagramNodeViewModelBase CreateDiagramNodeViewModel(IDiagramService diagramService, IDiagramNode diagramNode)
        {
            if (diagramNode is RoslynTypeDiagramNode roslynTypeDiagramNode)
                return new RoslynTypeDiagramNodeViewModel(ModelService, diagramService, roslynTypeDiagramNode, IsDescriptionVisible);

            throw new InvalidOperationException($"Unexpected diagram node type {diagramNode?.GetType().Name}");
        }
    }
}