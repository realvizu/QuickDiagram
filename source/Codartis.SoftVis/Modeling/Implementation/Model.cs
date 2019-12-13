using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Codartis.SoftVis.Graphs.Immutable;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.Modeling.Definition.Events;
using Codartis.Util;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Modeling.Implementation
{
    using IModelGraph = IImmutableBidirectionalGraph<IModelNode, ModelNodeId, IModelRelationship, ModelRelationshipId>;
    using ModelGraph = ImmutableBidirectionalGraph<IModelNode, ModelNodeId, IModelRelationship, ModelRelationshipId>;

    /// <summary>
    /// Implements an immutable model.
    /// </summary>
    public sealed class Model : IModel
    {
        private readonly IModelGraph _graph;
        [NotNull] private readonly ImmutableDictionary<object, IModelNode> _payloadToModelNodeMap;
        [NotNull] private readonly ImmutableDictionary<object, IModelRelationship> _payloadToModelRelationshipMap;
        [CanBeNull] [ItemNotNull] private readonly IEnumerable<IModelRuleProvider> _modelRuleProviders;
        [CanBeNull] private readonly IEqualityComparer<object> _nodePayloadEqualityComparer;
        [CanBeNull] private readonly IEqualityComparer<object> _relationshipPayloadEqualityComparer;

        private Model(
            IModelGraph graph,
            [NotNull] ImmutableDictionary<object, IModelNode> payloadToModelNodeMap,
            [NotNull] ImmutableDictionary<object, IModelRelationship> payloadToModelRelationshipMap,
            [CanBeNull] IEnumerable<IModelRuleProvider> modelRuleProviders,
            [CanBeNull] IEqualityComparer<object> nodePayloadEqualityComparer,
            [CanBeNull] IEqualityComparer<object> relationshipPayloadEqualityComparer)
        {
            _graph = graph;
            _payloadToModelNodeMap = payloadToModelNodeMap;
            _payloadToModelRelationshipMap = payloadToModelRelationshipMap;
            _modelRuleProviders = modelRuleProviders;
            _nodePayloadEqualityComparer = nodePayloadEqualityComparer;
            _relationshipPayloadEqualityComparer = relationshipPayloadEqualityComparer;
        }

        public IEnumerable<IModelNode> Nodes => _graph.Vertices;
        public IEnumerable<IModelRelationship> Relationships => _graph.Edges;

        public bool ContainsNode(ModelNodeId nodeId) => Nodes.Any(i => i.Id == nodeId);
        public bool ContainsRelationship(ModelRelationshipId relationshipId) => Relationships.Any(i => i.Id == relationshipId);

        public Maybe<IModelNode> TryGetNode(ModelNodeId nodeId) => _graph.TryGetVertex(nodeId);

        public Maybe<IModelNode> TryGetParentNode(ModelNodeId modelNodeId)
        {
            var parentNodes = GetRelatedNodes(modelNodeId, CommonDirectedModelRelationshipTypes.Container).ToList();

            if (parentNodes.Count > 1)
                throw new Exception($"There are {parentNodes.Count} parent nodes for node {modelNodeId}.");

            return Maybe.Create(parentNodes.SingleOrDefault());
        }

        public Maybe<IModelNode> TryGetNodeByPayload(object payload)
        {
            return _payloadToModelNodeMap.TryGetValue(payload, out var modelNode)
                ? Maybe.Create(modelNode)
                : Maybe<IModelNode>.Nothing;
        }

        public Maybe<IModelRelationship> TryGetRelationship(ModelRelationshipId relationshipId) => _graph.TryGetEdge(relationshipId);

        public Maybe<IModelRelationship> TryGetRelationshipByPayload(object payload)
        {
            return _payloadToModelRelationshipMap.TryGetValue(payload, out var modelRelationship)
                ? Maybe.Create(modelRelationship)
                : Maybe<IModelRelationship>.Nothing;
        }

        public IEnumerable<IModelNode> GetRelatedNodes(
            ModelNodeId nodeId,
            DirectedModelRelationshipType directedModelRelationshipType,
            bool recursive = false)
        {
            return _graph.GetAdjacentVertices(
                nodeId,
                directedModelRelationshipType.Direction,
                i => i.Stereotype == directedModelRelationshipType.Stereotype,
                recursive);
        }

        public IEnumerable<IModelRelationship> GetRelationships(ModelNodeId nodeId) => _graph.GetAllEdges(nodeId);

        public ModelEvent AddNode(
            string name,
            ModelNodeStereotype stereotype,
            object payload = null,
            ModelNodeId? parentNodeId = null)
        {
            if (payload != null && _payloadToModelNodeMap.ContainsKey(payload))
                throw new Exception($"The model already contains a node with payload: {payload}");

            var itemEvents = new List<ModelItemEventBase>();

            var newNode = CreateNode(name, stereotype, payload);
            var (newGraph, newPayloadToModelNodeMap) = AddNodeCore(newNode, _graph, _payloadToModelNodeMap, itemEvents);

            var newPayloadToModelRelationshipMap = _payloadToModelRelationshipMap;

            if (parentNodeId.HasValue)
            {
                var containsRelationship = CreateRelationship(parentNodeId.Value, newNode.Id, ModelRelationshipStereotype.Containment, payload: null);
                (newGraph, newPayloadToModelRelationshipMap) = AddRelationshipCore(
                    containsRelationship,
                    newGraph,
                    newPayloadToModelRelationshipMap,
                    itemEvents);
            }

            var newModel = CreateInstance(newGraph, newPayloadToModelNodeMap, newPayloadToModelRelationshipMap);
            return ModelEvent.Create(newModel, itemEvents);
        }

        public ModelEvent UpdateNode(IModelNode updatedNode)
        {
            var itemEvents = new List<ModelItemEventBase>();

            var (newGraph, newPayloadToModelNodeMap) = UpdateNodeCore(updatedNode, _graph, _payloadToModelNodeMap, itemEvents);

            var newModel = CreateInstance(newGraph, newPayloadToModelNodeMap, _payloadToModelRelationshipMap);
            return ModelEvent.Create(newModel, itemEvents);
        }

        public ModelEvent RemoveNode(ModelNodeId nodeId)
        {
            var itemEvents = new List<ModelItemEventBase>();
            var newGraph = _graph;

            var newPayloadToModelRelationshipMap = _payloadToModelRelationshipMap;

            foreach (var relationship in GetRelationships(nodeId))
                (newGraph, newPayloadToModelRelationshipMap) = RemoveRelationshipCore(relationship.Id, newGraph, newPayloadToModelRelationshipMap, itemEvents);

            ImmutableDictionary<object, IModelNode> newPayloadToModelNodeMap;
            (newGraph, newPayloadToModelNodeMap) = RemoveNodeCore(nodeId, newGraph, _payloadToModelNodeMap, itemEvents);

            var newModel = CreateInstance(newGraph, newPayloadToModelNodeMap, newPayloadToModelRelationshipMap);
            return ModelEvent.Create(newModel, itemEvents);
        }

        public ModelEvent AddRelationship(
            ModelNodeId sourceId,
            ModelNodeId targetId,
            ModelRelationshipStereotype stereotype,
            object payload = null)
        {
            if (payload != null && _payloadToModelRelationshipMap.ContainsKey(payload))
                throw new Exception($"The model already contains a relationship with payload: {payload}");

            var relationship = CreateRelationship(sourceId, targetId, stereotype, payload);

            if (!IsRelationshipValid(relationship))
                throw new ArgumentException($"{relationship} is invalid.");

            var itemEvents = new List<ModelItemEventBase>();
            var (newGraph, newPayloadToModelRelationshipMap) = AddRelationshipCore(
                relationship,
                _graph,
                _payloadToModelRelationshipMap,
                itemEvents);

            var newModel = CreateInstance(newGraph, _payloadToModelNodeMap, newPayloadToModelRelationshipMap);
            return ModelEvent.Create(newModel, itemEvents);
        }

        public ModelEvent RemoveRelationship(ModelRelationshipId relationshipId)
        {
            var itemEvents = new List<ModelItemEventBase>();
            var (newGraph, newPayloadToModelRelationshipMap) = RemoveRelationshipCore(relationshipId, _graph, _payloadToModelRelationshipMap, itemEvents);

            var newModel = CreateInstance(newGraph, _payloadToModelNodeMap, newPayloadToModelRelationshipMap);
            return ModelEvent.Create(newModel, itemEvents);
        }

        public ModelEvent Clear()
        {
            var itemEvents = Relationships.Select(i => new ModelRelationshipRemovedEvent(i))
                .OfType<ModelItemEventBase>()
                .Concat(Nodes.Select(i => new ModelNodeRemovedEvent(i)));

            var newModel = Create(_modelRuleProviders, _nodePayloadEqualityComparer, _relationshipPayloadEqualityComparer);
            return ModelEvent.Create(newModel, itemEvents);
        }

        [NotNull]
        private static IModelNode CreateNode(
            [NotNull] string name,
            ModelNodeStereotype stereotype,
            [CanBeNull] object payload)
        {
            return new ModelNode(ModelNodeId.Create(), name, stereotype, payload);
        }

        [NotNull]
        private static IModelRelationship CreateRelationship(
            ModelNodeId sourceId,
            ModelNodeId targetId,
            ModelRelationshipStereotype stereotype,
            [CanBeNull] object payload)
        {
            return new ModelRelationship(ModelRelationshipId.Create(), sourceId, targetId, stereotype, payload);
        }

        private static (IModelGraph, ImmutableDictionary<object, IModelNode>) AddNodeCore(
            [NotNull] IModelNode newNode,
            [NotNull] IModelGraph modelGraph,
            [NotNull] ImmutableDictionary<object, IModelNode> payloadToModelNodeMap,
            [NotNull] [ItemNotNull] ICollection<ModelItemEventBase> itemEvents)
        {
            itemEvents.Add(new ModelNodeAddedEvent(newNode));

            return (
                modelGraph.AddVertex(newNode),
                newNode.Payload == null
                    ? payloadToModelNodeMap
                    : payloadToModelNodeMap.Add(newNode.Payload, newNode)
            );
        }

        private static (IModelGraph, ImmutableDictionary<object, IModelRelationship> payloadToModelRelationshipMap) AddRelationshipCore(
            [NotNull] IModelRelationship relationship,
            [NotNull] IModelGraph modelGraph,
            [NotNull] ImmutableDictionary<object, IModelRelationship> payloadToModelRelationshipMap,
            [NotNull] [ItemNotNull] ICollection<ModelItemEventBase> itemEvents)
        {
            itemEvents.Add(new ModelRelationshipAddedEvent(relationship));

            return (
                modelGraph.AddEdge(relationship),
                relationship.Payload == null
                    ? payloadToModelRelationshipMap
                    : payloadToModelRelationshipMap.Add(relationship.Payload, relationship)
            );
        }

        private (IModelGraph, ImmutableDictionary<object, IModelNode>) UpdateNodeCore(
            [NotNull] IModelNode updatedNode,
            [NotNull] IModelGraph modelGraph,
            [NotNull] ImmutableDictionary<object, IModelNode> payloadToModelNodeMap,
            [NotNull] [ItemNotNull] ICollection<ModelItemEventBase> itemEvents)
        {
            var oldNode = GetNode(updatedNode.Id);
            itemEvents.Add(new ModelNodeUpdatedEvent(oldNode, updatedNode));

            if (oldNode.Payload != null)
                payloadToModelNodeMap = payloadToModelNodeMap.Remove(oldNode.Payload);

            if (updatedNode.Payload != null)
                payloadToModelNodeMap = payloadToModelNodeMap.Add(updatedNode.Payload, updatedNode);

            return (
                modelGraph.UpdateVertex(updatedNode),
                payloadToModelNodeMap
            );
        }

        private (IModelGraph, ImmutableDictionary<object, IModelNode>) RemoveNodeCore(
            ModelNodeId nodeId,
            [NotNull] IModelGraph modelGraph,
            [NotNull] ImmutableDictionary<object, IModelNode> payloadToModelNodeMap,
            [NotNull] [ItemNotNull] ICollection<ModelItemEventBase> itemEvents)
        {
            var oldNode = GetNode(nodeId);
            itemEvents.Add(new ModelNodeRemovedEvent(oldNode));

            return (
                modelGraph.RemoveVertex(nodeId),
                oldNode.Payload == null
                    ? payloadToModelNodeMap
                    : payloadToModelNodeMap.Remove(oldNode.Payload)
            );
        }

        private (IModelGraph, ImmutableDictionary<object, IModelRelationship>) RemoveRelationshipCore(
            ModelRelationshipId relationshipId,
            [NotNull] IModelGraph modelGraph,
            [NotNull] ImmutableDictionary<object, IModelRelationship> payloadToModelRelationshipMap,
            [NotNull] [ItemNotNull] ICollection<ModelItemEventBase> itemEvents)
        {
            var oldRelationship = GetRelationship(relationshipId);
            itemEvents.Add(new ModelRelationshipRemovedEvent(oldRelationship));

            return (
                modelGraph.RemoveEdge(relationshipId),
                oldRelationship.Payload == null
                    ? payloadToModelRelationshipMap
                    : payloadToModelRelationshipMap.Remove(oldRelationship.Payload)
            );
        }

        [NotNull]
        private IModelNode GetNode(ModelNodeId nodeId) => _graph.GetVertex(nodeId);

        [NotNull]
        private IModelRelationship GetRelationship(ModelRelationshipId relationshipId) => _graph.GetEdge(relationshipId);

        private bool IsRelationshipValid([NotNull] IModelRelationship relationship)
        {
            var sourceNode = GetNode(relationship.Source);
            var targetNode = GetNode(relationship.Target);

            return _modelRuleProviders?.All(i => i.IsRelationshipStereotypeValid(relationship.Stereotype, sourceNode, targetNode)) == true;
        }

        [NotNull]
        private IModel CreateInstance(
            IModelGraph graph,
            [NotNull] ImmutableDictionary<object, IModelNode> payloadToModelNodeMap,
            [NotNull] ImmutableDictionary<object, IModelRelationship> payloadToModelRelationshipMap)
            => new Model(
                graph,
                payloadToModelNodeMap,
                payloadToModelRelationshipMap,
                _modelRuleProviders,
                _nodePayloadEqualityComparer,
                _relationshipPayloadEqualityComparer);

        [NotNull]
        public static IModel Create(
            [CanBeNull] IEnumerable<IModelRuleProvider> modelRuleProviders = null,
            [CanBeNull] IEqualityComparer<object> nodePayloadEqualityComparer = null,
            [CanBeNull] IEqualityComparer<object> relationshipPayloadEqualityComparer = null)
        {
            var payloadToModelNodeMap = ImmutableDictionary<object, IModelNode>.Empty;
            if (nodePayloadEqualityComparer != null)
                payloadToModelNodeMap = payloadToModelNodeMap.WithComparers(nodePayloadEqualityComparer);

            var payloadToModelRelationshipMap = ImmutableDictionary<object, IModelRelationship>.Empty;
            if (relationshipPayloadEqualityComparer != null)
                payloadToModelRelationshipMap = payloadToModelRelationshipMap.WithComparers(relationshipPayloadEqualityComparer);

            return new Model(
                ModelGraph.Empty(allowParallelEdges: false),
                payloadToModelNodeMap,
                payloadToModelRelationshipMap,
                modelRuleProviders,
                nodePayloadEqualityComparer,
                relationshipPayloadEqualityComparer);
        }
    }
}