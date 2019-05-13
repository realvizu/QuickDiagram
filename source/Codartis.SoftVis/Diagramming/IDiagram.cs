using System.Collections.Generic;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Diagramming
{
    /// <summary>
    /// A diagram is a partial, graphical representation of a model.
    /// Immutable.
    /// </summary>
    /// <remarks>
    /// A diagram shows a subset of the model and there can be many diagrams depicting different areas/aspects of the same model.
    /// A diagram consists of shapes that represent model items: diagram nodes for model nodes and diagram connectors for model relationships.
    /// Diagram nodes can form a hierarchy meaning model node containment.
    /// Diagram nodes + connectors form a directed graph.
    /// </remarks>
    public interface IDiagram
    {
        Rect2D ContentRect { get; }

        IEnumerable<IDiagramShape> Shapes { get; }
        IEnumerable<IDiagramNode> Nodes { get; }
        IEnumerable<IDiagramConnector> Connectors { get; }

        bool NodeExists(ModelNodeId modelNodeId);
        bool ConnectorExists(ModelRelationshipId modelRelationshipId);
        bool PathExists(ModelNodeId sourceModelNodeId, ModelNodeId targetModelNodeId);
        bool IsConnectorRedundant(ModelRelationshipId modelRelationshipId);

        IDiagramNode GetNode(ModelNodeId modelNodeId);
        bool TryGetNode(ModelNodeId modelNodeId, out IDiagramNode node);
        IDiagramConnector GetConnector(ModelRelationshipId modelRelationshipId);
        bool TryGetConnector(ModelRelationshipId modelRelationshipId, out IDiagramConnector connector);
        IEnumerable<IDiagramConnector> GetConnectorsByNode(ModelNodeId id);
        IEnumerable<IDiagramNode> GetAdjacentNodes(ModelNodeId id, DirectedModelRelationshipType? directedModelRelationshipType = null);

        IDiagram AddNode(IDiagramNode node, IContainerDiagramNode parentNode = null);
        IDiagram RemoveNode(ModelNodeId nodeId);
        IDiagram UpdateNode(IDiagramNode newNode);
        IDiagram AddConnector(IDiagramConnector connector);
        IDiagram RemoveConnector(ModelRelationshipId connectorId);
        IDiagram UpdateConnector(IDiagramConnector newConnector);
        IDiagram Clear();
    }
}