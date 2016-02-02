using System;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Graph;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Creates view models from diagram shapes.
    /// </summary>
    internal class DiagramShapeViewModelFactory : DiagramViewModelBase
    {
        private readonly IConnectorTypeResolver _connectorTypeResolver;

        public DiagramShapeViewModelFactory(IModel model, Diagram diagram, IConnectorTypeResolver connectorTypeResolver)
              : base(model, diagram)
        {
            _connectorTypeResolver = connectorTypeResolver;
        }

        public DiagramShapeViewModelBase CreateViewModel(DiagramShape diagramShape)
        {
            if (diagramShape is DiagramNode)
                return new DiagramNodeViewModel(Model, Diagram, (DiagramNode)diagramShape);

            if (diagramShape is DiagramConnector)
            {
                var diagramConnector = (DiagramConnector) diagramShape;
                var connectorType = _connectorTypeResolver.GetConnectorType(diagramConnector.ModelRelationship);
                return new DiagramConnectorViewModel(Model, Diagram, diagramConnector, connectorType);
            }

            throw new NotImplementedException();
        }
    }
}
