using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Diagramming.Definition.Layout;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.Util;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Implementation.Layout
{
    public sealed class DiagramLayoutStructure
    {
        [NotNull] private readonly IDiagram _diagram;
        [NotNull] private readonly IDictionary<ModelNodeId, ILayoutGroup> _nodeLayoutGroups;
        [NotNull] private readonly IImmutableDictionary<ModelRelationshipId, IDiagramConnector> _crossLayoutGroupConnectors;

        /// <summary>
        /// Gets the root level layout group of the diagram.
        /// </summary>
        [NotNull]
        public ILayoutGroup RootLayoutGroup { get; }

        /// <summary>
        /// Returns those connectors that cross between layout groups therefore doesn't belong to any of them.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        public IImmutableSet<IDiagramConnector> CrossLayoutGroupConnectors { get; }

        public DiagramLayoutStructure([NotNull] IDiagram diagram)
        {
            _diagram = diagram;
            _nodeLayoutGroups = CreateLayoutGroupForAllNodes();
            _crossLayoutGroupConnectors = GetCrossLayoutGroupConnectors();
            RootLayoutGroup = CreateLayoutGroup(Maybe<ModelNodeId>.Nothing);
            CrossLayoutGroupConnectors = _crossLayoutGroupConnectors.Values.ToImmutableHashSet();
        }

        /// <summary>
        /// Gets the layout group of a given node.
        /// </summary>
        public Maybe<ILayoutGroup> TryGetLayoutGroupByNodeId(ModelNodeId modelNodeId)
        {
            return _nodeLayoutGroups[modelNodeId].ToMaybe();
        }

        public bool IsCrossingLayoutGroups(ModelRelationshipId modelRelationshipId)
        {
            return _crossLayoutGroupConnectors.ContainsKey(modelRelationshipId);
        }

        [NotNull]
        private ImmutableDictionary<ModelNodeId, ILayoutGroup> CreateLayoutGroupForAllNodes()
        {
            return _diagram.Nodes
                .Select(i => (i.Id, CreateLayoutGroup(i.Id.ToMaybe())))
                .ToImmutableDictionary(i => i.Id, i => i.Item2);
        }

        [NotNull]
        private IImmutableDictionary<ModelRelationshipId, IDiagramConnector> GetCrossLayoutGroupConnectors()
        {
            return _diagram.Connectors
                .Where(i => _diagram.GetNode(i.Source).ParentNodeId != _diagram.GetNode(i.Target).ParentNodeId)
                .ToImmutableDictionary(i => i.Id);
        }

        [NotNull]
        private ILayoutGroup CreateLayoutGroup(Maybe<ModelNodeId> containerNodeId)
        {
            var nodesInLayoutGroup = _diagram.Nodes
                .Where(i => i.ParentNodeId.Equals(containerNodeId))
                .ToImmutableHashSet();

            var connectorsInLayoutGroup = _diagram.Connectors
                .Where(i => _diagram.GetNode(i.Source).ParentNodeId.Equals(containerNodeId) && _diagram.GetNode(i.Target).ParentNodeId.Equals(containerNodeId))
                .ToImmutableHashSet();

            return LayoutGroup.CreateForNode(containerNodeId, nodesInLayoutGroup, connectorsInLayoutGroup);
        }
    }
}