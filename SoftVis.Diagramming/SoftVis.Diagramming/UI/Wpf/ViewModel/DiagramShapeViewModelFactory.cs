using System;
using Codartis.SoftVis.Diagramming;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Creates view models from diagram shapes.
    /// </summary>
    internal class DiagramShapeViewModelFactory : DiagramViewModelBase
    {

        public DiagramShapeViewModelFactory(IDiagram diagram)
              : base(diagram)
        {
        }

        public DiagramShapeViewModelBase CreateViewModel(IDiagramShape diagramShape)
        {
            if (diagramShape is IDiagramNode)
                return new DiagramNodeViewModel(Diagram, (IDiagramNode)diagramShape);

            if (diagramShape is IDiagramConnector)
            {
                var diagramConnector = (IDiagramConnector) diagramShape;
                return new DiagramConnectorViewModel(Diagram, diagramConnector);
            }

            throw new NotImplementedException();
        }
    }
}
