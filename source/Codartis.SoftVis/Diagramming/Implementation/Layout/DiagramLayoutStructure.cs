using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Diagramming.Definition.Layout;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.Util;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Implementation.Layout
{
    /// <summary>
    /// Separates the diagram into node and connector groups that are laid out together.
    /// </summary>
    /// <remarks>
    /// A diagram' layout consists of layout groups that act like little diagrams that the main diagram is composed of.
    /// Each layout group consists of shapes that represent model items: diagram nodes for model nodes and diagram connectors for model relationships.
    /// A connector belongs to a layout group if both its source and target nodes are in that group.
    /// Those connectors whose source and target nodes belong to different layout groups form a special connector group:
    /// the CrossLayoutGroupConnectors, that have different layout rules than other layout groups.
    /// </remarks>
    public sealed class DiagramLayoutStructure
    {
        [NotNull] private readonly IDiagram _diagram;
        [NotNull] private readonly IDictionary<ModelNodeId, ILayoutGroup> _nodeLayoutGroups;
        [NotNull] private readonly IDictionary<ModelRelationshipId, IDiagramConnector> _crossLayoutGroupConnectors;

        /// <summary>
        /// Gets the root level layout group of the diagram.
        /// </summary>
        [NotNull]
        public ILayoutGroup RootLayoutGroup { get; }

        public DiagramLayoutStructure([NotNull] IDiagram diagram)
        {
            _diagram = diagram;
            _nodeLayoutGroups = CreateLayoutGroupForAllNodes();
            _crossLayoutGroupConnectors = GetCrossLayoutGroupConnectors();
            RootLayoutGroup = CreateLayoutGroup(Maybe<ModelNodeId>.Nothing);
        }

        [NotNull] [ItemNotNull] public IEnumerable<IDiagramConnector> CrossLayoutGroupConnectors => _crossLayoutGroupConnectors.Values;

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
        private IDictionary<ModelNodeId, ILayoutGroup> CreateLayoutGroupForAllNodes()
        {
            return _diagram.Nodes
                .Select(i => (i.Id, CreateLayoutGroup(i.Id.ToMaybe())))
                .ToDictionary(i => i.Id, i => i.Item2);
        }

        [NotNull]
        private IDictionary<ModelRelationshipId, IDiagramConnector> GetCrossLayoutGroupConnectors()
        {
            return _diagram.Connectors
                .Where(i => _diagram.GetNode(i.Source).ParentNodeId != _diagram.GetNode(i.Target).ParentNodeId)
                .ToDictionary(i => i.Id);
        }

        [NotNull]
        private ILayoutGroup CreateLayoutGroup(Maybe<ModelNodeId> containerNodeId)
        {
            var nodesInLayoutGroup = _diagram.Nodes
                .Where(i => i.ParentNodeId.Equals(containerNodeId));

            var connectorsInLayoutGroup = _diagram.Connectors
                .Where(i => _diagram.GetNode(i.Source).ParentNodeId.Equals(containerNodeId) && _diagram.GetNode(i.Target).ParentNodeId.Equals(containerNodeId));

            return LayoutGroup.CreateForNode(containerNodeId, nodesInLayoutGroup, connectorsInLayoutGroup);
        }
    }
}