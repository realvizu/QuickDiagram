using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
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

        public IDiagramNode AddNode(ModelNodeId nodeId, ModelNodeId? parentNodeId = null)
        {
            lock (_diagramUpdateLockObject)
            {
                var maybeDiagramNode = LatestDiagram.TryGetNode(nodeId);
                if (maybeDiagramNode.HasValue)
                    return maybeDiagramNode.Value;

                var maybeModelNode = LatestDiagram.Model.TryGetNode(nodeId);
                if (!maybeModelNode.HasValue)
                    throw new Exception($"Node {nodeId} not found in model.");

                var newNode = CreateNode(maybeModelNode.Value).WithParentNodeId(parentNodeId.ToMaybe());
                LatestDiagram = LatestDiagram.AddNode(newNode);
                DiagramChanged?.Invoke(new DiagramNodeAddedEvent(LatestDiagram, newNode));

                return newNode;
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

        public IDiagramConnector AddConnector(ModelRelationshipId relationshipId)
        {
            lock (_diagramUpdateLockObject)
            {
                var maybeConnector = LatestDiagram.TryGetConnector(relationshipId);
                if (maybeConnector.HasValue)
                    return maybeConnector.Value;

                var maybeRelationship = LatestDiagram.Model.TryGetRelationship(relationshipId);
                if (!maybeRelationship.HasValue)
                    throw new InvalidOperationException($"Relationship {relationshipId} does not exist.");

                var connector = CreateConnector(maybeRelationship.Value);
                LatestDiagram = LatestDiagram.AddConnector(connector);
                DiagramChanged?.Invoke(new DiagramConnectorAddedEvent(LatestDiagram, connector));
                return connector;
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

        public IReadOnlyList<IDiagramNode> AddNodes(
            IEnumerable<ModelNodeId> modelNodeIds,
            CancellationToken cancellationToken,
            IIncrementalProgress progress)
        {
            var diagramNodes = new List<IDiagramNode>();

            foreach (var modelNodeId in modelNodeIds)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var diagramNode = AddNode(modelNodeId);
                diagramNodes.Add(diagramNode);

                progress?.Report(1);
            }

            return diagramNodes;
        }

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