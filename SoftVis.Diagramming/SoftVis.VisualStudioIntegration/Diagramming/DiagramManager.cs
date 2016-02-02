using System;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Graph;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.VisualStudioIntegration.Events;

namespace Codartis.SoftVis.VisualStudioIntegration.Diagramming
{
    /// <summary>
    /// Encapsulates a diagram and its builder.
    /// </summary>
    internal class DiagramManager : IDiagramServices
    {
        private readonly RoslynBasedDiagramBuilder _diagramBuilder;

        public Diagram Diagram { get; }

        public event EventHandler PackageEvent;

        public DiagramManager()
        {
            var connectorTypeResolver = new RoslynBasedConnectorTypeResolver();
            Diagram = new RoslynBasedDiagram(connectorTypeResolver);
            Diagram.ShapeSelected += OnShapeSelected;
            Diagram.ShapeActivated += OnShapeActivated;

            _diagramBuilder = new RoslynBasedDiagramBuilder(Diagram);
        }

        public void ClearDiagram()
        {
            Diagram.Clear();
        }

        public void ShowModelEntity(IModelEntity modelEntity)
        {
            _diagramBuilder.ShowModelEntity(modelEntity);
        }

        public void ShowModelEntityWithRelatedEntities(IModelEntity modelEntity)
        {
            _diagramBuilder.ShowModelEntityWithRelatedEntities(modelEntity);
        }

        private void OnShapeSelected(object sender, DiagramShape diagramShape)
        {
            // TODO
        }

        private void OnShapeActivated(object sender, DiagramShape diagramShape)
        {
            if (diagramShape is DiagramNode)
            {
                var eventArgs = new DiagramNodeActivatedEventArgs((DiagramNode) diagramShape);
                PackageEvent?.Invoke(sender, eventArgs);
            }
        }
    }
}
