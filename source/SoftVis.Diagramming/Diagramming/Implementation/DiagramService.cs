using System;
using System.Collections.Generic;
using System.Threading;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Util;

namespace Codartis.SoftVis.Diagramming.Implementation
{
    /// <summary>
    /// Implements diagram-related operations.
    /// </summary>
    public abstract class DiagramService : IDiagramService
    {
        protected DiagramStore DiagramStore { get; }
        protected IModelService ModelService { get; }
        protected IDiagramShapeFactory DiagramShapeFactory { get; }

        protected DiagramService(IDiagram diagram, IModelService modelService, IDiagramShapeFactory diagramShapeFactory)
        {
            DiagramStore = new DiagramStore(diagram);
            ModelService = modelService;
            DiagramShapeFactory = diagramShapeFactory;
        }

        public IDiagram Diagram => DiagramStore.Diagram;

        public event Action<DiagramEventBase> DiagramChanged
        {
            add => DiagramStore.DiagramChanged += value;
            remove => DiagramStore.DiagramChanged -= value;
        }

        public void AddNode(IDiagramNode node) => DiagramStore.AddNode(node);
        public void RemoveNode(IDiagramNode node) => DiagramStore.RemoveNode(node);
        public void UpdateDiagramNodeModelNode(IDiagramNode diagramNode, IModelNode newModelNode) => DiagramStore.UpdateDiagramNodeModelNode(diagramNode, newModelNode);
        public void UpdateDiagramNodeSize(IDiagramNode diagramNode, Size2D newSize) => DiagramStore.UpdateDiagramNodeSize(diagramNode, newSize);
        public void UpdateDiagramNodeCenter(IDiagramNode diagramNode, Point2D newCenter) => DiagramStore.UpdateDiagramNodeCenter(diagramNode, newCenter);
        public void AddConnector(IDiagramConnector connector) => DiagramStore.AddConnector(connector);
        public void RemoveConnector(IDiagramConnector connector) => DiagramStore.RemoveConnector(connector);
        public void UpdateDiagramConnectorRoute(IDiagramConnector connector, Route newRoute) => DiagramStore.UpdateDiagramConnectorRoute(connector, newRoute);
        public void ClearDiagram() => DiagramStore.ClearDiagram();

        public abstract ConnectorType GetConnectorType(ModelRelationshipStereotype modelRelationshipStereotype);

        public IDiagramNode GetDiagramNodeById(ModelNodeId id) => Diagram.GetNodeById(id);

        public IDiagramNode ShowModelNode(IModelNode modelNode)
        {
            var diagram = DiagramStore.Diagram;
            if (diagram.TryGetNodeById(modelNode.Id, out var existingDiagramNode))
                return existingDiagramNode;

            var diagramNode = DiagramShapeFactory.CreateDiagramNode(this, modelNode);
            DiagramStore.AddNode(diagramNode);
            return diagramNode;
        }

        public IReadOnlyList<IDiagramNode> ShowModelNodes(IEnumerable<IModelNode> modelNodes, 
            CancellationToken cancellationToken, IIncrementalProgress progress)
        {
            var diagramNodes = new List<IDiagramNode>();

            foreach (var modelNode in modelNodes)
            {
                if (cancellationToken.IsCancellationRequested)
                    return Array.Empty<IDiagramNode>();

                var diagramNode = ShowModelNode(modelNode);
                diagramNodes.Add(diagramNode);

                progress?.Report(1);
            }

            return diagramNodes;
        }

        public void HideModelNode(IModelNode modelNode)
        {
            var diagram = DiagramStore.Diagram;
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
            var diagram = DiagramStore.Diagram;
            if (diagram.ConnectorExistsById(modelRelationship.Id))
                return;

            var diagramConnector = DiagramShapeFactory.CreateDiagramConnector(this, modelRelationship);
            DiagramStore.AddConnector(diagramConnector);
        }

        public void HideModelRelationship(IModelRelationship modelRelationship)
        {
            var diagram = DiagramStore.Diagram;
            if (diagram.TryGetConnectorById(modelRelationship.Id, out IDiagramConnector diagramConnector))
                DiagramStore.RemoveConnector(diagramConnector);
        }

        public Rect2D GetRect(IEnumerable<ModelNodeId> modelNodeIds)
        {
            var diagram = Diagram;

            var nodeRects = new List<Rect2D>();
            foreach (var modelNodeId in modelNodeIds)
            {
                if (diagram.TryGetNodeById(modelNodeId, out var diagramNode))
                    nodeRects.Add(diagramNode.Rect);
            }

            return nodeRects.Union();
        }
    }
}
