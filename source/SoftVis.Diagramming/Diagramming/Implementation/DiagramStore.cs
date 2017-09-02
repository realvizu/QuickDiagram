using System;
using Codartis.SoftVis.Diagramming.Events;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Diagramming.Implementation
{
    /// <summary>
    /// Implements a diagram mutator.
    /// Keeps a current diagram version, implements mutator operations and publishes change events.
    /// The underlying model is immutable so each modification creates a new snapshot of the diagram.
    /// </summary>
    /// <remarks>
    /// Mutators must not run concurrently. A lock ensures it.
    /// </remarks>
    public sealed class DiagramStore : IDiagramMutator
    {
        public IDiagram Diagram { get; private set; }

        private readonly object _diagramUpdateLockObject = new object();

        public event Action<DiagramEventBase> DiagramChanged;

        public DiagramStore(IDiagram diagram)
        {
            Diagram = diagram ?? throw new ArgumentNullException(nameof(diagram));
        }

        public void AddNode(IDiagramNode node)
        {
            lock (_diagramUpdateLockObject)
            {
                if (Diagram.NodeExistsById(node.Id))
                    return;

                Diagram = Diagram.AddNode(node);
                DiagramChanged?.Invoke(new DiagramNodeAddedEvent(Diagram, node));
            }
        }

        public void RemoveNode(IDiagramNode node)
        {
            lock (_diagramUpdateLockObject)
            {
                if (!Diagram.NodeExistsById(node.Id))
                    return;

                Diagram = Diagram.RemoveNode(node);
                DiagramChanged?.Invoke(new DiagramNodeRemovedEvent(Diagram, node));
            }
        }

        public void UpdateDiagramNodeModelNode(IDiagramNode diagramNode, IModelNode newModelNode)
        {
            lock (_diagramUpdateLockObject)
            {
                if (!Diagram.TryGetNodeById(diagramNode.Id, out IDiagramNode oldNode))
                    return;

                var newNode = oldNode.WithModelNode(newModelNode);
                Diagram = Diagram.ReplaceNode(oldNode, newNode);
                DiagramChanged?.Invoke(new DiagramNodeModelNodeChangedEvent(Diagram, oldNode, newNode));
            }
        }

        public void UpdateDiagramNodeSize(IDiagramNode diagramNode, Size2D newSize)
        {
            lock (_diagramUpdateLockObject)
            {
                if (!Diagram.TryGetNodeById(diagramNode.Id, out IDiagramNode oldNode))
                    return;

                var newNode = oldNode.WithSize(newSize);
                Diagram = Diagram.ReplaceNode(oldNode, newNode);
                DiagramChanged?.Invoke(new DiagramNodeSizeChangedEvent(Diagram, oldNode, newNode));
            }
        }

        public void UpdateDiagramNodeCenter(IDiagramNode diagramNode, Point2D newCenter)
        {
            lock (_diagramUpdateLockObject)
            {
                if (!Diagram.TryGetNodeById(diagramNode.Id, out IDiagramNode oldNode))
                    return;

                var newNode = oldNode.WithCenter(newCenter);
                Diagram = Diagram.ReplaceNode(oldNode, newNode);
                DiagramChanged?.Invoke(new DiagramNodePositionChangedEvent(Diagram, oldNode, newNode));
            }
        }

        public void AddConnector(IDiagramConnector connector)
        {
            lock (_diagramUpdateLockObject)
            {
                if (Diagram.ConnectorExistsById(connector.Id))
                    return;

                Diagram = Diagram.AddConnector(connector);
                DiagramChanged?.Invoke(new DiagramConnectorAddedEvent(Diagram, connector));
            }
        }

        public void RemoveConnector(IDiagramConnector connector)
        {
            lock (_diagramUpdateLockObject)
            {
                if (!Diagram.ConnectorExistsById(connector.Id))
                    return;

                Diagram = Diagram.RemoveConnector(connector);
                DiagramChanged?.Invoke(new DiagramConnectorRemovedEvent(Diagram, connector));
            }
        }

        public void UpdateDiagramConnectorRoute(IDiagramConnector diagramConnector, Route newRoute)
        {
            lock (_diagramUpdateLockObject)
            {
                if (!Diagram.TryGetConnectorById(diagramConnector.Id, out IDiagramConnector oldConnector))
                    return;

                var newConnector = oldConnector.WithRoute(newRoute);
                Diagram = Diagram.ReplaceConnector(oldConnector, newConnector);
                DiagramChanged?.Invoke(new DiagramConnectorRouteChangedEvent(Diagram, oldConnector, newConnector));
            }
        }

        public void ClearDiagram()
        {
            lock (_diagramUpdateLockObject)
            {
                Diagram = Diagram.Clear();
                DiagramChanged?.Invoke(new DiagramClearedEvent(Diagram));
            }
        }
    }
}
