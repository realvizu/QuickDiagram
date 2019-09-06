using System;
using System.Diagnostics;
using Codartis.SoftVis.Diagramming.Events;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.Util;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Implementation
{
    /// <summary>
    /// Implements diagram-related operations.
    /// </summary>
    public class DiagramService : IDiagramService
    {
        public IDiagram LatestDiagram { get; private set; }

        [NotNull] private readonly IConnectorTypeResolver _connectorTypeResolver;
        [NotNull] private readonly object _diagramUpdateLockObject;

        public event Action<DiagramEventBase> DiagramChanged;

        public DiagramService([NotNull] IModel model, [NotNull] IConnectorTypeResolver connectorTypeResolver)
        {
            LatestDiagram = Diagram.Create(model);
            _connectorTypeResolver = connectorTypeResolver;
            _diagramUpdateLockObject = new object();
        }

        public void AddNode(ModelNodeId nodeId, ModelNodeId? parentNodeId)
        {
            lock (_diagramUpdateLockObject)
            {
                if (LatestDiagram.NodeExists(nodeId))
                    return;

                LatestDiagram.Model.TryGetNode(nodeId).Match(
                    modelNode =>
                    {
                        var newNode = CreateNode(modelNode).WithParentNodeId(parentNodeId.ToMaybe());
                        LatestDiagram = LatestDiagram.AddNode(newNode);
                        DiagramChanged?.Invoke(new DiagramNodeAddedEvent(LatestDiagram, newNode));
                    },
                    () => throw new Exception($"Node {nodeId} not found in model."));
            }
        }

        public void RemoveNode(ModelNodeId nodeId)
        {
            lock (_diagramUpdateLockObject)
            {
                if (!LatestDiagram.NodeExists(nodeId))
                    return;

                var oldNode = LatestDiagram.GetNode(nodeId);
                LatestDiagram = LatestDiagram.RemoveNode(nodeId);
                DiagramChanged?.Invoke(new DiagramNodeRemovedEvent(LatestDiagram, oldNode));
            }
        }

        public void UpdateDiagramNodeModelNode(IDiagramNode diagramNode, IModelNode newModelNode)
        {
            lock (_diagramUpdateLockObject)
            {
                LatestDiagram
                    .TryGetNode(diagramNode.Id)
                    .Match(
                        oldNode =>
                        {
                            var updatedNode = oldNode.WithModelNode(newModelNode);
                            LatestDiagram = LatestDiagram.UpdateNode(updatedNode);
                            DiagramChanged?.Invoke(new DiagramNodeModelNodeChangedEvent(LatestDiagram, oldNode, updatedNode));
                        },
                        () => throw new InvalidOperationException($"Node {diagramNode} does not exist."));
            }
        }

        public void UpdateDiagramNodeSize(IDiagramNode diagramNode, Size2D newSize)
        {
            lock (_diagramUpdateLockObject)
            {
                LatestDiagram
                    .TryGetNode(diagramNode.Id)
                    .Match(
                        oldNode =>
                        {
                            var updatedNode = oldNode.WithSize(newSize);
                            LatestDiagram = LatestDiagram.UpdateNode(updatedNode);
                            DiagramChanged?.Invoke(new DiagramNodeSizeChangedEvent(LatestDiagram, oldNode, updatedNode));
                        },
                        () => throw new InvalidOperationException($"Node {diagramNode} does not exist."));
            }
        }

        public void UpdateDiagramNodeCenter(IDiagramNode diagramNode, Point2D newCenter)
        {
            lock (_diagramUpdateLockObject)
            {
                LatestDiagram
                    .TryGetNode(diagramNode.Id)
                    .Match(
                        oldNode =>
                        {
                            var updatedNode = oldNode.WithCenter(newCenter);
                            LatestDiagram = LatestDiagram.UpdateNode(updatedNode);
                            DiagramChanged?.Invoke(new DiagramNodePositionChangedEvent(LatestDiagram, oldNode, updatedNode));
                        },
                        () => Debug.WriteLine($"Trying to move node {diagramNode} but it does not exist."));
            }
        }

        public void UpdateDiagramNodeTopLeft(IDiagramNode diagramNode, Point2D newTopLeft)
        {
            lock (_diagramUpdateLockObject)
            {
                LatestDiagram
                    .TryGetNode(diagramNode.Id)
                    .Match(
                        oldNode =>
                        {
                            var updatedNode = oldNode.WithTopLeft(newTopLeft);
                            LatestDiagram = LatestDiagram.UpdateNode(updatedNode);
                            DiagramChanged?.Invoke(new DiagramNodePositionChangedEvent(LatestDiagram, oldNode, updatedNode));
                        },
                        () => throw new InvalidOperationException($"Node {diagramNode} does not exist."));
            }
        }

        public void AddConnector(ModelRelationshipId relationshipId)
        {
            lock (_diagramUpdateLockObject)
            {
                if (LatestDiagram.ConnectorExists(relationshipId))
                    return;

                LatestDiagram.Model.TryGetRelationship(relationshipId).Match(
                    relationship =>
                    {
                        var connector = CreateConnector(relationship);
                        LatestDiagram = LatestDiagram.AddConnector(connector);
                        DiagramChanged?.Invoke(new DiagramConnectorAddedEvent(LatestDiagram, connector));
                    },
                    () => throw new InvalidOperationException($"Relationship {relationshipId} does not exist."));
            }
        }

        public void RemoveConnector(ModelRelationshipId relationshipId)
        {
            lock (_diagramUpdateLockObject)
            {
                if (!LatestDiagram.ConnectorExists(relationshipId))
                    return;

                var oldConnector = LatestDiagram.GetConnector(relationshipId);
                LatestDiagram = LatestDiagram.RemoveConnector(relationshipId);
                DiagramChanged?.Invoke(new DiagramConnectorRemovedEvent(LatestDiagram, oldConnector));
            }
        }

        public void UpdateConnectorRoute(ModelRelationshipId relationshipId, Route newRoute)
        {
            lock (_diagramUpdateLockObject)
            {
                LatestDiagram
                    .TryGetConnector(relationshipId)
                    .Match(
                        oldConnector =>
                        {
                            var newConnector = oldConnector.WithRoute(newRoute);
                            LatestDiagram = LatestDiagram.UpdateConnector(newConnector);
                            DiagramChanged?.Invoke(new DiagramConnectorRouteChangedEvent(LatestDiagram, oldConnector, newConnector));
                        },
                        () => throw new InvalidOperationException($"Connector {relationshipId} does not exist."));
            }
        }

        public void ClearDiagram()
        {
            lock (_diagramUpdateLockObject)
            {
                LatestDiagram = LatestDiagram.Clear();
                DiagramChanged?.Invoke(new DiagramClearedEvent(LatestDiagram));
            }
        }

        public ConnectorType GetConnectorType(ModelRelationshipStereotype stereotype)
        {
            return _connectorTypeResolver.GetConnectorType(stereotype);
        }

        //public IDiagramNode ShowModelNode(IModelNode modelNode)
        //{
        //    return DiagramStore.LatestDiagram.TryGetNode(modelNode.Id).Match(
        //        node => node,
        //        () => AddNode(modelNode)
        //    );
        //}

        //private IDiagramNode AddNode(IModelNode modelNode)
        //{
        //   var maybeParentModelNode =  ModelService.LatestModel.TryGetParentNode(modelNode.Id);

        //    var diagramNode = new DiagramNode(modelNode, maybeParentModelNode.FromMaybe());
        //    DiagramStore.AddNode(diagramNode);
        //    return diagramNode;
        //}

        //public IReadOnlyList<IDiagramNode> ShowModelNodes(
        //    IEnumerable<IModelNode> modelNodes,
        //    CancellationToken cancellationToken,
        //    IIncrementalProgress progress)
        //{
        //    var diagramNodes = new List<IDiagramNode>();

        //    foreach (var modelNode in modelNodes)
        //    {
        //        cancellationToken.ThrowIfCancellationRequested();

        //        var diagramNode = ShowModelNode(modelNode);
        //        diagramNodes.Add(diagramNode);

        //        progress?.Report(1);
        //    }

        //    return diagramNodes;
        //}

        //public void HideModelNode(ModelNodeId modelNodeId)
        //{
        //    var diagram = DiagramStore.LatestDiagram;
        //    if (diagram.NodeExists(modelNodeId))
        //        RemoveNode(modelNodeId, diagram);
        //}

        //private void RemoveNode(ModelNodeId modelNodeId, IDiagram diagram)
        //{
        //    var diagramConnectors = diagram.GetConnectorsByNode(modelNodeId);
        //    foreach (var diagramConnector in diagramConnectors)
        //        DiagramStore.RemoveConnector(diagramConnector.Id);

        //    DiagramStore.RemoveNode(modelNodeId);
        //}

        //public void ShowModelRelationship(IModelRelationship modelRelationship)
        //{
        //    if (modelRelationship.Stereotype == ModelRelationshipStereotype.Containment)
        //        return;

        //    var diagram = DiagramStore.LatestDiagram;
        //    if (diagram.ConnectorExists(modelRelationship.Id))
        //        return;

        //    var diagramConnectorSpec = DiagramShapeFactory.CreateDiagramConnector(modelRelationship);
        //    DiagramStore.AddConnector(diagramConnectorSpec);
        //}

        //public void HideModelRelationship(ModelRelationshipId modelRelationshipId)
        //{
        //    var diagram = DiagramStore.LatestDiagram;
        //    if (diagram.ConnectorExists(modelRelationshipId))
        //        DiagramStore.RemoveConnector(modelRelationshipId);
        //}

        [NotNull]
        private static IDiagramNode CreateNode([NotNull] IModelNode modelNode) => new DiagramNode(modelNode);

        [NotNull]
        private IDiagramConnector CreateConnector([NotNull] IModelRelationship relationship)
        {
            var connectorType = _connectorTypeResolver.GetConnectorType(relationship.Stereotype);
            return new DiagramConnector(relationship, connectorType);
        }
    }
}