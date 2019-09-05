﻿using System;
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

        [NotNull]private readonly object _diagramUpdateLockObject;

        public event Action<DiagramEventBase> DiagramChanged;

        public DiagramService([NotNull] IModel model)
        {
            LatestDiagram = Diagram.Create(model);
            _diagramUpdateLockObject = new object();
        }

        public void AddNode(IDiagramNode node)
        {
            lock (_diagramUpdateLockObject)
            {
                if (LatestDiagram.NodeExists(node.Id))
                    return;

                LatestDiagram = LatestDiagram.AddNode(node);
                DiagramChanged?.Invoke(new DiagramNodeAddedEvent(LatestDiagram, node));
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

        public void AddConnector([NotNull] IDiagramConnector connector)
        {
            lock (_diagramUpdateLockObject)
            {
                if (LatestDiagram.ConnectorExists(connector.Id))
                    return;

                LatestDiagram = LatestDiagram.AddConnector(connector);
                DiagramChanged?.Invoke(new DiagramConnectorAddedEvent(LatestDiagram, connector));
            }
        }

        public void RemoveConnector(ModelRelationshipId connectorId)
        {
            lock (_diagramUpdateLockObject)
            {
                if (!LatestDiagram.ConnectorExists(connectorId))
                    return;

                var oldConnector = LatestDiagram.GetConnector(connectorId);
                LatestDiagram = LatestDiagram.RemoveConnector(connectorId);
                DiagramChanged?.Invoke(new DiagramConnectorRemovedEvent(LatestDiagram, oldConnector));
            }
        }

        public void UpdateConnectorRoute(ModelRelationshipId connectorId, Route newRoute)
        {
            lock (_diagramUpdateLockObject)
            {
                LatestDiagram
                    .TryGetConnector(connectorId)
                    .Match(
                        oldConnector =>
                        {
                            var newConnector = oldConnector.WithRoute(newRoute);
                            LatestDiagram = LatestDiagram.UpdateConnector(newConnector);
                            DiagramChanged?.Invoke(new DiagramConnectorRouteChangedEvent(LatestDiagram, oldConnector, newConnector));
                        },
                        () => throw new InvalidOperationException($"Connector {connectorId} does not exist."));
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

        public ConnectorType GetConnectorType(ModelRelationshipStereotype modelRelationshipStereotype)
        {
            return ConnectorTypes.Dependency;
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

        public ConnectorType GetConnectorType(string stereotype)
        {
            throw new NotImplementedException();
        }
    }
}