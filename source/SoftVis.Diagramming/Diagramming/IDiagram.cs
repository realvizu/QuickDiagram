using System.Collections.Generic;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Diagramming
{
    /// <summary>
    /// A diagram is a partial, graphical representation of a model.
    /// Immutable.
    /// A diagram consists of shapes that represent model items: Diagram nodes for model nodes and diagram connectors for model relatioships.
    /// The diagram nodes and connectors form a directed graph.
    /// </summary>
    /// <remarks>
    /// A diagram shows a subset of the model and there can be many diagrams depicting different areas/aspects of the same model.
    /// </remarks>
    public interface IDiagram
    {
        Rect2D ContentRect { get; }

        IEnumerable<IDiagramShape> Shapes { get; }
        IEnumerable<IDiagramNode> Nodes { get; }
        IEnumerable<IDiagramConnector> Connectors { get; }

        bool NodeExists(IDiagramNode node);
        bool NodeExistsById(ModelNodeId modelNodeId);
        bool ConnectorExists(IDiagramConnector connector);
        bool ConnectorExistsById(ModelRelationshipId modelRelationshipId);
        bool PathExists(IDiagramNode sourceNode, IDiagramNode targetNode);
        bool PathExistsById(ModelNodeId sourceModelNodeId, ModelNodeId targetModelNodeIdNode);

        IDiagramNode GetNodeById(ModelNodeId modelNodeId);
        IDiagramConnector GetConnectorById(ModelRelationshipId modelRelationshipId);

        IDiagram AddNode(IDiagramNode node);
        IDiagram RemoveNode(IDiagramNode node);
        IDiagram ReplaceNode(IDiagramNode oldNode, IDiagramNode newNode);
        IDiagram AddConnector(IDiagramConnector connector);
        IDiagram RemoveConnector(IDiagramConnector connector);
        IDiagram ReplaceConnector(IDiagramConnector oldConnector, IDiagramConnector newConnector);
        IDiagram Clear();
    }
}