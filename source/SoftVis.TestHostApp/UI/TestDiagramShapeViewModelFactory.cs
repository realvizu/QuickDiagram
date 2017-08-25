using System;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.TestHostApp.Diagramming;
using Codartis.SoftVis.UI.Wpf.ViewModel;

namespace Codartis.SoftVis.TestHostApp.UI
{
    public class TestDiagramShapeViewModelFactory : DiagramShapeViewModelFactoryBase
    {
        public TestDiagramShapeViewModelFactory(IArrangedDiagram diagram)
            : base(diagram)
        {
        }

        public override DiagramShapeViewModelBase CreateViewModel(IDiagramShape diagramShape)
        {
            switch (diagramShape)
            {
                case TypeDiagramNode typeDiagramNode:

                case IDiagramConnector diagramConnector:
                    return CreateDiagramConnectorViewModel(diagramConnector);

                default:
                    throw new NotImplementedException();
            }
        }

        public override DiagramNodeViewModelBase CreateDiagramNodeViewModel(IDiagramNode diagramNode)
        {
            return new TypeDiagramNodeViewModel(Diagram, typeDiagramNode);
        }

        public override DiagramConnectorViewModel CreateDiagramConnectorViewModel(IDiagramConnector diagramConnector)
        {
            var sourceNode = DiagramShapeViewModelRepository.GetDiagramNodeViewModel(diagramConnector.Source);
            var targetNode = DiagramShapeViewModelRepository.GetDiagramNodeViewModel(diagramConnector.Target);
            return new DiagramConnectorViewModel(Diagram, diagramConnector, sourceNode, targetNode);
        }
    }
}