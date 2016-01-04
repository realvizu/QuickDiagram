using System;
using Codartis.SoftVis.Diagramming;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Creates view models from diagram shapes.
    /// </summary>
    internal class DiagramShapeViewModelFactory
    {
        private readonly IConnectorTypeResolver _connectorTypeResolver;

        public DiagramShapeViewModelFactory(IConnectorTypeResolver connectorTypeResolver)
        {
            _connectorTypeResolver = connectorTypeResolver;
        }

        public DiagramShapeViewModelBase CreateViewModel(DiagramShape diagramShape)
        {
            if (diagramShape is DiagramNode)
                return new DiagramNodeViewModel2((DiagramNode)diagramShape);

            if (diagramShape is DiagramConnector)
            {
                var diagramConnector = (DiagramConnector) diagramShape;
                var connectorType = _connectorTypeResolver.GetConnectorType(diagramConnector.ModelRelationship);
                return new DiagramConnectorViewModel2(diagramConnector, connectorType);
            }

            throw new NotImplementedException();
        }

        public void UpdateViewModel(DiagramShapeViewModelBase diagramShapeViewModel)
        {
            diagramShapeViewModel.UpdateState();
        }
    }
}
