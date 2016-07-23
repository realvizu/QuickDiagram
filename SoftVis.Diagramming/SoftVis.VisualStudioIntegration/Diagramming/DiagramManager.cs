using System;
using Codartis.SoftVis.Diagramming;
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

        public IDiagram Diagram { get; }

        public event EventHandler PackageEvent;
        public event EventHandler<IDiagramShape> ShapeAddedToDiagram; 

        public DiagramManager()
        {
            var connectorTypeResolver = new RoslynBasedConnectorTypeResolver();
            Diagram = new RoslynBasedDiagram(connectorTypeResolver);
            Diagram.ShapeSelected += OnShapeSelected;
            Diagram.ShapeActivated += OnShapeActivated;
            Diagram.ShapeAdded += OnShapeAdded;

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

        private void OnShapeSelected(object sender, IDiagramShape diagramShape)
        {
            // TODO
        }

        private void OnShapeActivated(object sender, IDiagramShape diagramShape)
        {
            if (diagramShape is IDiagramNode)
            {
                var eventArgs = new DiagramNodeActivatedEventArgs((IDiagramNode) diagramShape);
                PackageEvent?.Invoke(sender, eventArgs);
            }
        }

        private void OnShapeAdded(object sender, IDiagramShape diagramShape)
        {
            ShapeAddedToDiagram?.Invoke(sender, diagramShape);
        }
    }
}
