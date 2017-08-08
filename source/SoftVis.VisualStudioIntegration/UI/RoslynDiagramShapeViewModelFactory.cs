using System;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.UI.Wpf.ViewModel;

namespace Codartis.SoftVis.VisualStudioIntegration.UI
{
    /// <summary>
    /// Creates view models for roslyn-based diagram shapes.
    /// </summary>
    public class RoslynDiagramShapeViewModelFactory : DiagramShapeViewModelFactoryBase
    {
        public RoslynDiagramShapeViewModelFactory(IArrangedDiagram diagram)
            : base(diagram)
        {
        }

        public override DiagramShapeViewModelBase CreateViewModel(IDiagramShape diagramShape, bool isDescriptionVisible)
        {
            switch (diagramShape)
            {
                case IDiagramNode diagramNode:
                    return new TypeDiagramNodeViewModel(Diagram, (IDiagramNode)diagramShape, isDescriptionVisible);

                case IDiagramConnector diagramConnector:
                    return CreateDiagramConnectorViewModel(diagramConnector);

                default:
                    throw new NotImplementedException();
            }
        }

        private DiagramShapeViewModelBase CreateDiagramConnectorViewModel(IDiagramConnector diagramConnector)
        {
            var sourceNode = DiagramShapeViewModelRepository.GetDiagramNodeViewModel(diagramConnector.Source);
            var targetNode = DiagramShapeViewModelRepository.GetDiagramNodeViewModel(diagramConnector.Target);
            return new DiagramConnectorViewModel(Diagram, diagramConnector, sourceNode, targetNode);
        }
    }
}