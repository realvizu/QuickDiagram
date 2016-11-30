using System;
using System.Linq;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Util.UI.Wpf.Collections;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Creates view models from diagram shapes.
    /// </summary>
    internal class DiagramShapeViewModelFactory : DiagramViewModelBase
    {
        private readonly ObservableImmutableList<DiagramNodeViewModel> _diagramNodeViewModels;

        public DiagramShapeViewModelFactory(IArrangedDiagram diagram, ObservableImmutableList<DiagramNodeViewModel> diagramNodeViewModels)
              : base(diagram)
        {
            _diagramNodeViewModels = diagramNodeViewModels;
        }

        public DiagramShapeViewModelBase CreateViewModel(IDiagramShape diagramShape)
        {
            if (diagramShape is IDiagramNode)
                return new DiagramNodeViewModel(Diagram, (IDiagramNode)diagramShape);

            if (diagramShape is IDiagramConnector)
            {
                var diagramConnector = (IDiagramConnector) diagramShape;
                var sourceNode = _diagramNodeViewModels.First(i => i.DiagramShape == diagramConnector.Source);
                var targetNode = _diagramNodeViewModels.First(i => i.DiagramShape == diagramConnector.Target);
                return new DiagramConnectorViewModel(Diagram, diagramConnector, sourceNode, targetNode);
            }

            throw new NotImplementedException();
        }
    }
}
