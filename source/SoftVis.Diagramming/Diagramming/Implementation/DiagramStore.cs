using System;
using System.Linq;
using Codartis.SoftVis.Diagramming.Events;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Diagramming.Implementation
{
    /// <summary>
    /// Abstract base class for diagram stores.
    /// Responsible for diagram modification logic.
    /// </summary>
    public abstract class DiagramStore : IDiagramStore
    {
        public IDiagram CurrentDiagram { get; protected set; }

        protected readonly object DiagramUpdateLockObject = new object();

        public event Action<DiagramEventBase> DiagramChanged;

        public abstract ConnectorType GetConnectorType(ModelRelationshipStereotype modelRelationshipStereotype);

        protected DiagramStore(Diagram diagram)
        {
            CurrentDiagram = diagram ?? throw new ArgumentNullException(nameof(diagram));
        }

        public void AddNode(IDiagramNode node)
        {
            lock (DiagramUpdateLockObject)
            {
                CurrentDiagram = CurrentDiagram.AddNode(node);
                DiagramChanged?.Invoke(new DiagramNodeAddedEvent(CurrentDiagram, node));
            }
        }

        public void RemoveNode(IDiagramNode node)
        {
            lock (DiagramUpdateLockObject)
            {
                CurrentDiagram = CurrentDiagram.RemoveNode(node);
                DiagramChanged?.Invoke(new DiagramNodeRemovedEvent(CurrentDiagram, node));
            }
        }

        public void UpdateDiagramNodeModelNode(IDiagramNode diagramNode, IModelNode newModelNode)
        {
            lock (DiagramUpdateLockObject)
            {
                var oldNode = CurrentDiagram.GetNodeById(diagramNode.Id);
                var newNode = oldNode.WithModelNode(newModelNode);
                CurrentDiagram = CurrentDiagram.ReplaceNode(oldNode, newNode);
                DiagramChanged?.Invoke(new DiagramNodeModelNodeChangedEvent(CurrentDiagram, oldNode, newNode));
            }
        }

        public void UpdateDiagramNodeSize(IDiagramNode diagramNode, Size2D newSize)
        {
            lock (DiagramUpdateLockObject)
            {
                var oldNode = CurrentDiagram.GetNodeById(diagramNode.Id);
                var newNode = oldNode.WithSize(newSize);
                CurrentDiagram = CurrentDiagram.ReplaceNode(oldNode, newNode);
                DiagramChanged?.Invoke(new DiagramNodeSizeChangedEvent(CurrentDiagram, oldNode, newNode));
            }
        }

        public void UpdateDiagramNodeCenter(IDiagramNode diagramNode, Point2D newCenter)
        {
            lock (DiagramUpdateLockObject)
            {
                var oldNode = CurrentDiagram.GetNodeById(diagramNode.Id);
                var newNode = oldNode.WithCenter(newCenter);
                CurrentDiagram = CurrentDiagram.ReplaceNode(oldNode, newNode);
                DiagramChanged?.Invoke(new DiagramNodePositionChangedEvent(CurrentDiagram, oldNode, newNode));
            }
        }

        public void AddConnector(IDiagramConnector connector)
        {
            lock (DiagramUpdateLockObject)
            {
                CurrentDiagram = CurrentDiagram.AddConnector(connector);
                DiagramChanged?.Invoke(new DiagramConnectorAddedEvent(CurrentDiagram, connector));
            }
        }

        public void RemoveConnector(IDiagramConnector connector)
        {
            lock (DiagramUpdateLockObject)
            {
                CurrentDiagram = CurrentDiagram.RemoveConnector(connector);
                DiagramChanged?.Invoke(new DiagramConnectorRemovedEvent(CurrentDiagram, connector));
            }
        }

        public void UpdateDiagramConnectorRoute(IDiagramConnector diagramConnector, Route newRoute)
        {
            lock (DiagramUpdateLockObject)
            {
                var oldConnector = CurrentDiagram.GetConnectorById(diagramConnector.Id);
                var newConnector = oldConnector.WithRoute(newRoute);
                CurrentDiagram = CurrentDiagram.ReplaceConnector(oldConnector, newConnector);
                DiagramChanged?.Invoke(new DiagramConnectorRouteChangedEvent(CurrentDiagram, oldConnector, newConnector));
            }
        }

        public void ClearDiagram()
        {
            lock (DiagramUpdateLockObject)
            {
                CurrentDiagram = CurrentDiagram.Clear();
                DiagramChanged?.Invoke(new DiagramClearedEvent(CurrentDiagram));
            }
        }

        public IDiagramNode GetDiagramNodeByModelNode(IModelNode modelNode)
        {
            return CurrentDiagram.Nodes.FirstOrDefault(i => i.ModelNode == modelNode);
        }
    }
}
