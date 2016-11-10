using System;
using System.Collections.ObjectModel;
using System.Linq;
using Codartis.SoftVis.Diagramming;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Creates view models from diagram shapes.
    /// </summary>
    internal class DiagramShapeViewModelFactory : DiagramViewModelBase
    {
        private readonly ObservableCollection<DiagramShapeViewModelBase> _diagramShapeViewModels;

        public DiagramShapeViewModelFactory(IArrangedDiagram diagram,
            ObservableCollection<DiagramShapeViewModelBase> diagramShapeViewModels)
              : base(diagram)
        {
            _diagramShapeViewModels = diagramShapeViewModels;
        }

        public DiagramShapeViewModelBase CreateViewModel(IDiagramShape diagramShape)
        {
            if (diagramShape is IDiagramNode)
                return new DiagramNodeViewModel(Diagram, (IDiagramNode)diagramShape);

            if (diagramShape is IDiagramConnector)
            {
                var diagramConnector = (IDiagramConnector) diagramShape;
                var sourceNode = _diagramShapeViewModels.First(i => i.DiagramShape == diagramConnector.Source) as DiagramNodeViewModel;
                var targetNode = _diagramShapeViewModels.First(i => i.DiagramShape == diagramConnector.Target) as DiagramNodeViewModel;
                return new DiagramConnectorViewModel(Diagram, diagramConnector, sourceNode, targetNode);
            }

            throw new NotImplementedException();
        }
    }
}
