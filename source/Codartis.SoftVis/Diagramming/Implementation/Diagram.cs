using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Codartis.SoftVis.Graphs;
using Codartis.SoftVis.Graphs.Immutable;
using Codartis.SoftVis.Modeling;
using Codartis.Util;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Implementation
{
    using IDiagramGraph = IImmutableBidirectionalGraph<IDiagramNode, ModelNodeId, IDiagramConnector, ModelRelationshipId>;
    using DiagramGraph = ImmutableBidirectionalGraph<IDiagramNode, ModelNodeId, IDiagramConnector, ModelRelationshipId>;

    /// <summary>
    /// An immutable implementation of a diagram.
    /// </summary>
    public sealed class Diagram : IDiagram
    {
        [NotNull] public static readonly IDiagram Empty = new Diagram(LayoutGroup.Empty(), ImmutableHashSet<IDiagramConnector>.Empty);

        [NotNull] public ILayoutGroup RootLayoutGroup { get; }
        [NotNull] public IImmutableSet<IDiagramConnector> CrossLayoutGroupConnectors { get; }
        [NotNull] private readonly IDiagramGraph _allShapesGraph;

        private Diagram(
            [NotNull] ILayoutGroup rootLayoutGroup,
            [NotNull] IImmutableSet<IDiagramConnector> crossLayoutGroupConnectors)
        {
            RootLayoutGroup = rootLayoutGroup;
            CrossLayoutGroupConnectors = crossLayoutGroupConnectors;

            _allShapesGraph = DiagramGraph.Empty();
        }

        public IImmutableSet<IDiagramNode> Nodes => RootLayoutGroup.NodesRecursive;
        public IImmutableSet<IDiagramConnector> Connectors => RootLayoutGroup.ConnectorsRecursive.Union(CrossLayoutGroupConnectors);

        public bool NodeExists(ModelNodeId modelNodeId) => Nodes.Any(i => i.Id == modelNodeId);
        public bool ConnectorExists(ModelRelationshipId modelRelationshipId) => Connectors.Any(i => i.Id == modelRelationshipId);

        public bool PathExists(ModelNodeId sourceModelNodeId, ModelNodeId targetModelNodeId)
            => NodeExists(sourceModelNodeId) && NodeExists(targetModelNodeId) && _allShapesGraph.PathExists(sourceModelNodeId, targetModelNodeId);

        public bool PathExists(Maybe<ModelNodeId> maybeSourceModelNodeId, Maybe<ModelNodeId> maybeTargetModelNodeId)
        {
            return maybeSourceModelNodeId.Match(
                sourceNodeId => maybeTargetModelNodeId.Match(
                    targetNodeId => PathExists(sourceNodeId, targetNodeId),
                    () => false),
                () => false);
        }

        public bool IsConnectorRedundant(ModelRelationshipId modelRelationshipId)
        {
            return TryGetConnector(modelRelationshipId).Match(
                connector => _allShapesGraph.IsEdgeRedundant(connector),
                () => false);
        }

        public IDiagramNode GetNode(ModelNodeId modelNodeId) => Nodes.Single(i => i.Id == modelNodeId);

        public Maybe<IDiagramNode> TryGetNode(ModelNodeId modelNodeId) => Nodes.SingleOrDefault(i => i.Id == modelNodeId).ToMaybe();

        public IDiagramConnector GetConnector(ModelRelationshipId modelRelationshipId) => Connectors.Single(i => i.Id == modelRelationshipId);

        public Maybe<IDiagramConnector> TryGetConnector(ModelRelationshipId modelRelationshipId)
            => Connectors.SingleOrDefault(i => i.Id == modelRelationshipId).ToMaybe();

        public IEnumerable<IDiagramConnector> GetConnectorsByNode(ModelNodeId id) => Connectors.Where(i => i.Source.Id == id || i.Target.Id == id);

        //public IEnumerable<IDiagramNode> GetAdjacentNodes(ModelNodeId id, DirectedModelRelationshipType? directedModelRelationshipType = null)
        //{
        //    IEnumerable<IDiagramNode> result;

        //    if (directedModelRelationshipType != null)
        //    {
        //        result = _allShapesGraph.GetAdjacentVertices(
        //            id,
        //            directedModelRelationshipType.Value.Direction,
        //            e => e.ModelRelationship.Stereotype == directedModelRelationshipType.Value.Stereotype);
        //    }
        //    else
        //    {
        //        result = _allShapesGraph.GetAdjacentVertices(id, EdgeDirection.In)
        //            .Union(_allShapesGraph.GetAdjacentVertices(id, EdgeDirection.Out));
        //    }

        //    return result;
        //}

        public IDiagram WithNode(IDiagramNode node, ModelNodeId? parentNodeId = null)
        {
            var updatedLayoutGroup = RootLayoutGroup.WithNode(node, parentNodeId);

            return CreateInstance(updatedLayoutGroup, CrossLayoutGroupConnectors);
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

        public IDiagram Clear() => CreateInstance(RootLayoutGroup.Clear(), CrossLayoutGroupConnectors.Clear());

        [NotNull]
        private static IDiagram CreateInstance(
            [NotNull] ILayoutGroup rootLayoutGroup,
            [NotNull] IImmutableSet<IDiagramConnector> crossLayoutGroupConnectors)
        {
            return new Diagram(rootLayoutGroup, crossLayoutGroupConnectors);
        }
    }
}