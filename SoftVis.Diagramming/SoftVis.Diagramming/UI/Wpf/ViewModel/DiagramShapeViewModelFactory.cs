using System;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.UI.Extensibility;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Creates view models from diagram shapes.
    /// </summary>
    internal class DiagramShapeViewModelFactory : DiagramViewModelBase
    {
        private readonly IDiagramBehaviourProvider _diagramBehaviourProvider;

        public DiagramShapeViewModelFactory(IDiagram diagram, IDiagramBehaviourProvider diagramBehaviourProvider)
              : base(diagram)
        {
            _diagramBehaviourProvider = diagramBehaviourProvider;
        }

        public DiagramShapeViewModelBase CreateViewModel(IDiagramShape diagramShape)
        {
            if (diagramShape is IDiagramNode)
                return new DiagramNodeViewModel(Model, Diagram, _diagramBehaviourProvider, (IDiagramNode)diagramShape);

            if (diagramShape is IDiagramConnector)
            {
                var diagramConnector = (IDiagramConnector) diagramShape;
                var connectorType = Diagram.GetConnectorType(diagramConnector.ModelRelationship);
                return new DiagramConnectorViewModel(Model, Diagram, diagramConnector, connectorType);
            }

            throw new NotImplementedException();
        }
    }
}
