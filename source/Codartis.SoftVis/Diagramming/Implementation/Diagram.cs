using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Graphs.Immutable;
using Codartis.SoftVis.Modeling.Definition;
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
        public IModel Model { get; }
        public ILayoutGroup RootLayoutGroup { get; }
        [NotNull] private readonly IImmutableDictionary<ModelRelationshipId, IDiagramConnector> _crossLayoutGroupConnectors;

        public IImmutableSet<IDiagramNode> Nodes { get; }
        public IImmutableSet<IDiagramConnector> Connectors { get; }
        [NotNull] private readonly IDiagramGraph _allShapesGraph;

        private Diagram(
            [NotNull] IModel model,
            [NotNull] ILayoutGroup rootLayoutGroup,
            [NotNull] IImmutableDictionary<ModelRelationshipId, IDiagramConnector> crossLayoutGroupConnectors)
        {
            Model = model;
            RootLayoutGroup = rootLayoutGroup;
            _crossLayoutGroupConnectors = crossLayoutGroupConnectors;

            Nodes = RootLayoutGroup.NodesRecursive;
            Connectors = RootLayoutGroup.ConnectorsRecursive.Union(CrossLayoutGroupConnectors);
            _allShapesGraph = DiagramGraph.Create(Nodes, Connectors);
        }

        public IImmutableSet<IDiagramConnector> CrossLayoutGroupConnectors => _crossLayoutGroupConnectors.Values.ToImmutableHashSet();

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

        public bool IsConnectorRedundant(ModelRelationshipId modelRelationshipId) => _allShapesGraph.IsEdgeRedundant(modelRelationshipId);

        public IDiagramNode GetNode(ModelNodeId modelNodeId) => Nodes.Single(i => i.Id == modelNodeId);

        public Maybe<IDiagramNode> TryGetNode(ModelNodeId modelNodeId) => Nodes.SingleOrDefault(i => i.Id == modelNodeId).ToMaybe();

        public IDiagramConnector GetConnector(ModelRelationshipId modelRelationshipId) => Connectors.Single(i => i.Id == modelRelationshipId);

        public Maybe<IDiagramConnector> TryGetConnector(ModelRelationshipId modelRelationshipId)
            => Connectors.SingleOrDefault(i => i.Id == modelRelationshipId).ToMaybe();

        public IEnumerable<IDiagramConnector> GetConnectorsByNode(ModelNodeId id) => Connectors.Where(i => i.Source == id || i.Target == id);

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

        public IDiagram WithModel(IModel newModel)
        {
            return CreateInstance(newModel, RootLayoutGroup, _crossLayoutGroupConnectors);
        }

        public IDiagram AddNode(IDiagramNode node, ModelNodeId? parentNodeId = null)
        {
            return CreateInstance(Model, RootLayoutGroup.AddNode(node, parentNodeId), _crossLayoutGroupConnectors);
        }

        public IDiagram UpdateNode(IDiagramNode updatedNode)
        {
            return CreateInstance(Model, RootLayoutGroup.UpdateNode(updatedNode), _crossLayoutGroupConnectors);
        }

        public IDiagram RemoveNode(ModelNodeId nodeId)
        {
            return CreateInstance(Model, RootLayoutGroup.RemoveNode(nodeId), _crossLayoutGroupConnectors);
        }

        public IDiagram AddConnector(IDiagramConnector connector)
        {
            return IsCrossingLayoutGroups(connector.Id)
                ? CreateInstance(Model, RootLayoutGroup, _crossLayoutGroupConnectors.Add(connector.Id, connector))
                : CreateInstance(Model, RootLayoutGroup.AddConnector(connector), _crossLayoutGroupConnectors);
        }

        public IDiagram UpdateConnector(IDiagramConnector updatedConnector)
        {
            return IsCrossingLayoutGroups(updatedConnector.Id)
                ? CreateInstance(Model, RootLayoutGroup, _crossLayoutGroupConnectors.SetItem(updatedConnector.Id, updatedConnector))
                : CreateInstance(Model, RootLayoutGroup.UpdateConnector(updatedConnector), _crossLayoutGroupConnectors);
        }

        public IDiagram RemoveConnector(ModelRelationshipId connectorId)
        {
            return CrossLayoutGroupConnectors.Any(i => i.Id == connectorId)
                ? CreateInstance(Model, RootLayoutGroup, _crossLayoutGroupConnectors.Remove(connectorId))
                : CreateInstance(Model, RootLayoutGroup.RemoveConnector(connectorId), _crossLayoutGroupConnectors);
        }

        public IDiagram Clear() => CreateInstance(Model, RootLayoutGroup.Clear(), _crossLayoutGroupConnectors.Clear());

        public bool IsCrossingLayoutGroups(ModelRelationshipId modelRelationshipId)
        {
            var connector = GetConnector(modelRelationshipId);
            return GetNode(connector.Source).ParentNodeId != GetNode(connector.Target).ParentNodeId;
        }

        public Maybe<IContainerDiagramNode> TryGetContainerNode(IDiagramNode diagramNode)
        {
            return diagramNode.ParentNodeId == null
                ? Maybe<IContainerDiagramNode>.Nothing
                : Maybe.Create((IContainerDiagramNode)GetNode(diagramNode.ParentNodeId.Value));
        }

        public Rect2D GetRect(IEnumerable<ModelNodeId> modelNodeIds)
        {
            return modelNodeIds
                .Select(i => TryGetNode(i).Match(j => j.Rect, () => Rect2D.Undefined))
                .Union();
        }

        [NotNull]
        private static IDiagram CreateInstance(
            [NotNull] IModel model,
            [NotNull] ILayoutGroup rootLayoutGroup,
            [NotNull] IImmutableDictionary<ModelRelationshipId, IDiagramConnector> crossLayoutGroupConnectors)
        {
            return new Diagram(model, rootLayoutGroup, crossLayoutGroupConnectors);
        }

        [NotNull]
        public static IDiagram Create([NotNull] IModel model)
        {
            return CreateInstance(model, LayoutGroup.Empty(), ImmutableDictionary<ModelRelationshipId, IDiagramConnector>.Empty);
        }
    }
}