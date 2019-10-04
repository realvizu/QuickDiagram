using System;
using System.Collections.Generic;
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
        [NotNull] [ItemNotNull] private readonly IModelRuleProvider[] _modelRuleProviders;

        private Model(IModelGraph graph, [NotNull] params IModelRuleProvider[] modelRuleProviders)
        {
            _graph = graph;
            _modelRuleProviders = modelRuleProviders;
        }

        public IEnumerable<IModelNode> Nodes => _graph.Vertices;
        public IEnumerable<IModelRelationship> Relationships => _graph.Edges;

        public IModelNode GetNode(ModelNodeId nodeId) => _graph.GetVertex(nodeId);
        public Maybe<IModelNode> TryGetNode(ModelNodeId nodeId) => _graph.TryGetVertex(nodeId);

        public Maybe<IModelNode> TryGetParentNode(ModelNodeId modelNodeId)
        {
            var parentNodes = GetRelatedNodes(modelNodeId, CommonDirectedModelRelationshipTypes.Container).ToList();

            if (parentNodes.Count > 1)
                throw new Exception($"There are {parentNodes.Count} parent nodes for node {modelNodeId}.");

            return Maybe.Create(parentNodes.SingleOrDefault());
        }

        public IModelRelationship GetRelationship(ModelRelationshipId relationshipId) => _graph.GetEdge(relationshipId);
        public Maybe<IModelRelationship> TryGetRelationship(ModelRelationshipId relationshipId) => _graph.TryGetEdge(relationshipId);

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
            var itemEvents = new List<ModelItemEventBase>();

            var newNode = CreateNode(name, stereotype, payload);
            var newGraph = AddNodeCore(newNode, _graph, itemEvents);

            if (parentNodeId.HasValue)
            {
                var containsRelationship = CreateRelationship(parentNodeId.Value, newNode.Id, ModelRelationshipStereotype.Containment, payload: null);
                newGraph = AddRelationshipCore(containsRelationship, newGraph, itemEvents);
            }

            var newModel = CreateInstance(newGraph);
            return ModelEvent.Create(newModel, itemEvents);
        }

        public ModelEvent RemoveNode(ModelNodeId nodeId)
        {
            var itemEvents = new List<ModelItemEventBase>();
            var newGraph = _graph;

            foreach (var relationship in GetRelationships(nodeId))
                newGraph = RemoveRelationshipCore(relationship.Id, newGraph, itemEvents);

            newGraph = RemoveNodeCore(nodeId, newGraph, itemEvents);

            var newModel = CreateInstance(newGraph);
            return ModelEvent.Create(newModel, itemEvents);
        }

        public ModelEvent AddRelationship(
            ModelNodeId sourceId,
            ModelNodeId targetId,
            ModelRelationshipStereotype stereotype,
            object payload = null)
        {
            var relationship = CreateRelationship(sourceId, targetId, stereotype, payload);

            if (!IsRelationshipValid(relationship))
                throw new ArgumentException($"{relationship} is invalid.");

            var itemEvents = new List<ModelItemEventBase>();
            var newGraph = AddRelationshipCore(relationship, _graph, itemEvents);

            var newModel = CreateInstance(newGraph);
            return ModelEvent.Create(newModel, itemEvents);
        }

        public ModelEvent RemoveRelationship(ModelRelationshipId relationshipId)
        {
            var itemEvents = new List<ModelItemEventBase>();
            var newGraph = RemoveRelationshipCore(relationshipId, _graph, itemEvents);

            var newModel = CreateInstance(newGraph);
            return ModelEvent.Create(newModel, itemEvents);
        }

        public ModelEvent Clear()
        {
            var newModel = Create();
            // Shall we raise node and relationship removed events ?
            return ModelEvent.Create(newModel);
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

        [NotNull]
        private static IModelGraph AddNodeCore(
            [NotNull] IModelNode newNode,
            [NotNull] IModelGraph modelGraph,
            [NotNull] [ItemNotNull] ICollection<ModelItemEventBase> itemEvents)
        {
            itemEvents.Add(new ModelNodeAddedEvent(newNode));
            return modelGraph.AddVertex(newNode);
        }

        [NotNull]
        private static IModelGraph AddRelationshipCore(
            [NotNull] IModelRelationship relationship,
            [NotNull] IModelGraph modelGraph,
            [NotNull] [ItemNotNull] ICollection<ModelItemEventBase> itemEvents)
        {
            itemEvents.Add(new ModelRelationshipAddedEvent(relationship));
            return modelGraph.AddEdge(relationship);
        }

        [NotNull]
        private IModelGraph RemoveNodeCore(
            ModelNodeId nodeId,
            [NotNull] IModelGraph modelGraph,
            [NotNull] [ItemNotNull] ICollection<ModelItemEventBase> itemEvents)
        {
            var oldNode = GetNode(nodeId);
            itemEvents.Add(new ModelNodeRemovedEvent(oldNode));
            return modelGraph.RemoveVertex(nodeId);
        }

        [NotNull]
        private IModelGraph RemoveRelationshipCore(
            ModelRelationshipId relationshipId,
            [NotNull] IModelGraph modelGraph,
            [NotNull] [ItemNotNull] ICollection<ModelItemEventBase> itemEvents)
        {
            var oldRelationship = GetRelationship(relationshipId);
            itemEvents.Add(new ModelRelationshipRemovedEvent(oldRelationship));
            return modelGraph.RemoveEdge(relationshipId);
        }

        private bool IsRelationshipValid([NotNull] IModelRelationship relationship)
        {
            var sourceNode = GetNode(relationship.Source);
            var targetNode = GetNode(relationship.Target);

            return _modelRuleProviders.All(i => i.IsRelationshipStereotypeValid(relationship.Stereotype, sourceNode, targetNode));
        }

        [NotNull]
        private IModel CreateInstance(IModelGraph graph) => new Model(graph, _modelRuleProviders);

        [NotNull]
        public static IModel Create([NotNull] params IModelRuleProvider[] modelRuleProviders)
        {
            return new Model(ModelGraph.Empty(allowParallelEdges: false), modelRuleProviders);
        }
    }
}