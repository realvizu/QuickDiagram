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
        [NotNull] private readonly IDictionary<ModelNodeId, ILayoutGroup> _nodeLayoutGroups;
        [NotNull] private readonly IDictionary<ModelRelationshipId, IDiagramConnector> _crossLayoutGroupConnectors;

        /// <summary>
        /// Gets the root level layout group of the diagram.
        /// </summary>
        [NotNull]
        public ILayoutGroup RootLayoutGroup { get; }

        public DiagramLayoutStructure([NotNull] IDiagram diagram)
        {
            _nodeLayoutGroups = CreateLayoutGroupForAllNodes(diagram);
            _crossLayoutGroupConnectors = diagram.GetCrossLayoutGroupConnectors();
            RootLayoutGroup = diagram.CreateLayoutGroup(Maybe<ModelNodeId>.Nothing);
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
        private static IDictionary<ModelNodeId, ILayoutGroup> CreateLayoutGroupForAllNodes([NotNull] IDiagram diagram)
        {
            return diagram.Nodes
                .Select(i => (i.Id, diagram.CreateLayoutGroup(i.Id.ToMaybe())))
                .ToDictionary(i => i.Id, i => i.Item2);
        }
    }
}