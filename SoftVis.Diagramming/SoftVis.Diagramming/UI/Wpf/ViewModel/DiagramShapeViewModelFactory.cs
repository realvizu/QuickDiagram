using System;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Creates view models from diagram shapes.
    /// </summary>
    internal class DiagramShapeViewModelFactory : DiagramViewModelBase
    {
        private readonly IConnectorTypeResolver _connectorTypeResolver;

        public DiagramShapeViewModelFactory(IModel model, IDiagram diagram, IConnectorTypeResolver connectorTypeResolver)
              : base(model, diagram)
        {
            _connectorTypeResolver = connectorTypeResolver;
        }

        public DiagramShapeViewModelBase CreateViewModel(IDiagramShape diagramShape)
        {
            if (diagramShape is IDiagramNode)
                return new DiagramNodeViewModel(Model, Diagram, (IDiagramNode)diagramShape);

            if (diagramShape is IDiagramConnector)
            {
                var diagramConnector = (IDiagramConnector) diagramShape;
                var connectorType = _connectorTypeResolver.GetConnectorType(diagramConnector.ModelRelationship);
                return new DiagramConnectorViewModel(Model, Diagram, diagramConnector, connectorType);
            }

            throw new NotImplementedException();
        }
    }
}
