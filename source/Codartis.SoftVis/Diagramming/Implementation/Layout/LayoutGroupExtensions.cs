using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Diagramming.Definition.Layout;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.Util;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Implementation.Layout
{
    public static class LayoutGroupExtensions
    {
        [NotNull]
        public static IDictionary<ModelRelationshipId, IDiagramConnector> GetCrossLayoutGroupConnectors([NotNull] this IDiagram diagram)
        {
            return diagram.Connectors
                .Where(i => diagram.GetNode(i.Source).ParentNodeId != diagram.GetNode(i.Target).ParentNodeId)
                .ToDictionary(i => i.Id);
        }

        [NotNull]
        public static ILayoutGroup CreateLayoutGroup([NotNull] this IDiagram diagram, Maybe<ModelNodeId> containerNodeId)
        {
            var nodesInLayoutGroup = diagram.Nodes
                .Where(i => i.ParentNodeId.Equals(containerNodeId))
                .Where(i => i.Size.IsDefined);

            var connectorsInLayoutGroup = diagram.Connectors
                .Where(i => diagram.GetNode(i.Source).ParentNodeId.Equals(containerNodeId) && diagram.GetNode(i.Target).ParentNodeId.Equals(containerNodeId));

            return LayoutGroup.Create(nodesInLayoutGroup, connectorsInLayoutGroup);
        }
    }
}