using System;
using System.Collections.Generic;
using System.Threading;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Util;

namespace Codartis.SoftVis.Diagramming.Implementation
{
    /// <summary>
    /// Implement diagram-related operations.
    /// </summary>
    public class DiagramService : IDiagramService
    {
        protected IDiagramStore DiagramStore { get; }
        protected IDiagramShapeFactory DiagramShapeFactory { get; }

        public DiagramService(IDiagramStore diagramStore, IDiagramShapeFactory diagramShapeFactory)
        {
            DiagramStore = diagramStore;
            DiagramShapeFactory = diagramShapeFactory;
        }

        public IDiagram CurrentDiagram => DiagramStore.CurrentDiagram;

        public event Action<DiagramEventBase> DiagramChanged
        {
            add => DiagramStore.DiagramChanged += value;
            remove => DiagramStore.DiagramChanged -= value;
        }

        public ConnectorType GetConnectorType(ModelRelationshipStereotype stereotype) => DiagramStore.GetConnectorType(stereotype);
        public IDiagramNode GetDiagramNodeById(ModelNodeId id) => DiagramStore.GetDiagramNodeById(id);

        public void AddNode(IDiagramNode node) => DiagramStore.AddNode(node);
        public void RemoveNode(IDiagramNode node) => DiagramStore.RemoveNode(node);
        public void UpdateDiagramNodeModelNode(IDiagramNode diagramNode, IModelNode newModelNode) => DiagramStore.UpdateDiagramNodeModelNode(diagramNode, newModelNode);
        public void UpdateDiagramNodeSize(IDiagramNode diagramNode, Size2D newSize) => DiagramStore.UpdateDiagramNodeSize(diagramNode, newSize);
        public void UpdateDiagramNodeCenter(IDiagramNode diagramNode, Point2D newCenter) => DiagramStore.UpdateDiagramNodeCenter(diagramNode, newCenter);
        public void AddConnector(IDiagramConnector connector) => DiagramStore.AddConnector(connector);
        public void RemoveConnector(IDiagramConnector connector) => DiagramStore.RemoveConnector(connector);
        public void UpdateDiagramConnectorRoute(IDiagramConnector connector, Route newRoute) => DiagramStore.UpdateDiagramConnectorRoute(connector, newRoute);
        public void ClearDiagram() => DiagramStore.ClearDiagram();

        public void ShowModelNode(IModelNode modelNode)
        {
            var diagram = DiagramStore.CurrentDiagram;
            if (diagram.NodeExistsById(modelNode.Id))
                return;

            var diagramNode = DiagramShapeFactory.CreateDiagramNode(DiagramStore, modelNode);
            DiagramStore.AddNode(diagramNode);
        }

        public void ShowModelNodes(IEnumerable<IModelNode> modelNodes, CancellationToken cancellationToken, IIncrementalProgress progress)
        {
            foreach (var modelNode in modelNodes)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                ShowModelNode(modelNode);
                progress.Report(1);
            }
        }

        public void HideModelNode(IModelNode modelNode)
        {
            var diagram = DiagramStore.CurrentDiagram;
            if (diagram.TryGetNodeById(modelNode.Id, out IDiagramNode diagramNode))
            {
                var diagramConnectors = diagram.GetConnectorsByNodeId(modelNode.Id);
                foreach (var diagramConnector in diagramConnectors)
                    DiagramStore.RemoveConnector(diagramConnector);

                DiagramStore.RemoveNode(diagramNode);
            }
        }

        public void ShowModelRelationship(IModelRelationship modelRelationship)
        {
            var diagram = DiagramStore.CurrentDiagram;
            if (diagram.ConnectorExistsById(modelRelationship.Id))
                return;

            var diagramConnector = DiagramShapeFactory.CreateDiagramConnector(DiagramStore, modelRelationship);
            DiagramStore.AddConnector(diagramConnector);
        }

        public void HideModelRelationship(IModelRelationship modelRelationship)
        {
            var diagram = DiagramStore.CurrentDiagram;
            if (diagram.TryGetConnectorById(modelRelationship.Id, out IDiagramConnector diagramConnector))
                DiagramStore.RemoveConnector(diagramConnector);
        }
    }
}
