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

        public DiagramShapeViewModelFactory(IReadOnlyModel readOnlyModel, IDiagram diagram, IConnectorTypeResolver connectorTypeResolver)
              : base(readOnlyModel, diagram)
        {
            _connectorTypeResolver = connectorTypeResolver;
        }

        public DiagramShapeViewModelBase CreateViewModel(IDiagramShape diagramShape)
        {
            if (diagramShape is IDiagramNode)
                return new DiagramNodeViewModel(ReadOnlyModel, Diagram, (IDiagramNode)diagramShape);

            if (diagramShape is IDiagramConnector)
            {
                var diagramConnector = (IDiagramConnector) diagramShape;
                var connectorType = _connectorTypeResolver.GetConnectorType(diagramConnector.ModelRelationship);
                return new DiagramConnectorViewModel(ReadOnlyModel, Diagram, diagramConnector, connectorType);
            }

            throw new NotImplementedException();
        }
    }
}
