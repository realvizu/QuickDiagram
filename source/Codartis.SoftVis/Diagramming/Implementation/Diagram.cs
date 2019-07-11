using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Codartis.SoftVis.Graphs;
using Codartis.SoftVis.Modeling;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Implementation
{
    /// <summary>
    /// An immutable implementation of a diagram.
    /// </summary>
    public sealed class Diagram : IDiagram
    {
        [NotNull] public static readonly IDiagram Empty = new Diagram(new DiagramGraph(), LayoutGroup.Empty(), ImmutableHashSet<IDiagramConnector>.Empty);

        /// <summary>
        /// Contains all nodes and connectors in the diagram regardless of node hierarchy and layout groups.
        /// </summary>
        [NotNull]
        private readonly DiagramGraph _allShapesGraph;

        [NotNull] public ILayoutGroup RootLayoutGroup { get; }
        [NotNull] public IImmutableSet<IDiagramConnector> CrossLayoutGroupConnectors { get; }

        private Diagram(
            [NotNull] DiagramGraph allShapesGraph,
            [NotNull] ILayoutGroup rootLayoutGroup,
            [NotNull] IImmutableSet<IDiagramConnector> crossLayoutGroupConnectors)
        {
            _allShapesGraph = allShapesGraph;
            RootLayoutGroup = rootLayoutGroup;
            CrossLayoutGroupConnectors = crossLayoutGroupConnectors;
        }

        public IImmutableSet<IDiagramNode> Nodes => _allShapesGraph.Vertices.ToImmutableHashSet();
        public IImmutableSet<IDiagramConnector> Connectors => _allShapesGraph.Edges.ToImmutableHashSet();

        public bool NodeExists(ModelNodeId modelNodeId) => _allShapesGraph.ContainsVertex(modelNodeId);
        public bool ConnectorExists(ModelRelationshipId modelRelationshipId) => _allShapesGraph.ContainsEdge(modelRelationshipId);

        public bool PathExists(ModelNodeId sourceModelNodeId, ModelNodeId targetModelNodeId)
            => NodeExists(sourceModelNodeId) && NodeExists(targetModelNodeId) && _allShapesGraph.PathExists(sourceModelNodeId, targetModelNodeId);

        public bool IsConnectorRedundant(ModelRelationshipId modelRelationshipId)
            => TryGetConnector(modelRelationshipId, out var connector) && _allShapesGraph.IsEdgeRedundant(connector);

        public IDiagramNode GetNode(ModelNodeId modelNodeId) => _allShapesGraph.GetVertex(modelNodeId);
        public bool TryGetNode(ModelNodeId modelNodeId, out IDiagramNode node) => _allShapesGraph.TryGetVertex(modelNodeId, out node);

        public IDiagramConnector GetConnector(ModelRelationshipId modelRelationshipId) => _allShapesGraph.GetEdge(modelRelationshipId);

        public bool TryGetConnector(ModelRelationshipId modelRelationshipId, out IDiagramConnector connector)
            => _allShapesGraph.TryGetEdge(modelRelationshipId, out connector);

        public IEnumerable<IDiagramConnector> GetConnectorsByNode(ModelNodeId id) => Connectors.Where(i => i.Source.Id == id || i.Target.Id == id);

        public IEnumerable<IDiagramNode> GetAdjacentNodes(ModelNodeId id, DirectedModelRelationshipType? directedModelRelationshipType = null)
        {
            IEnumerable<IDiagramNode> result;

            if (directedModelRelationshipType != null)
            {
                result = _allShapesGraph.GetAdjacentVertices(
                    id,
                    directedModelRelationshipType.Value.Direction,
                    e => e.ModelRelationship.Stereotype == directedModelRelationshipType.Value.Stereotype);
            }
            else
            {
                result = _allShapesGraph.GetAdjacentVertices(id, EdgeDirection.In)
                    .Union(_allShapesGraph.GetAdjacentVertices(id, EdgeDirection.Out));
            }

            return result;
        }

        public IDiagram WithNode(IDiagramNode node, ModelNodeId? parentNodeId = null)
        {
            var updatedGraph = _allShapesGraph.AddVertex(node);
            var updatedLayoutGroup = RootLayoutGroup.WithNode(node, parentNodeId);

            return CreateInstance(updatedGraph, updatedLayoutGroup, CrossLayoutGroupConnectors);
        }

        public IDiagram WithoutNode(ModelNodeId nodeId)
        {
            throw new System.NotImplementedException();
        }

        public IDiagram WithConnector(IDiagramConnector connector)
        {
            throw new System.NotImplementedException();
        }

        public IDiagram WithoutConnector(ModelRelationshipId connectorId)
        {
            throw new System.NotImplementedException();
        }

        public IDiagram Clear() => CreateInstance(_allShapesGraph.Clear(), RootLayoutGroup.Clear(), CrossLayoutGroupConnectors.Clear());

        [NotNull]
        private static IDiagram CreateInstance(
            [NotNull] DiagramGraph allShapesGraph,
            [NotNull] ILayoutGroup rootLayoutGroup,
            [NotNull] IImmutableSet<IDiagramConnector> crossLayoutGroupConnectors)
        {
            return new Diagram(allShapesGraph, rootLayoutGroup, crossLayoutGroupConnectors);
        }
    }
}