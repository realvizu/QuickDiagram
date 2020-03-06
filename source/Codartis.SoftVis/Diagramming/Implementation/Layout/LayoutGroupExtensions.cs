using System;
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
        public static IEnumerable<IDiagramConnector> GetCrossLayoutGroupConnectors([NotNull] this IDiagram diagram)
        {
            return diagram.Connectors
                .Where(i => diagram.GetNode(i.Source).ParentNodeId != diagram.GetNode(i.Target).ParentNodeId);
        }

        [NotNull]
        public static ILayoutGroup CreateLayoutGroup(
            [NotNull] this IDiagram diagram,
            Maybe<ModelNodeId> containerNodeId,
            Predicate<IDiagramConnector> connectorFilter = null)
        {
            var nodesInLayoutGroup = diagram.Nodes
                .Where(i => i.ParentNodeId.Equals(containerNodeId))
                .Where(i => i.Size.IsDefined);

            var connectorsInLayoutGroup = diagram.Connectors
                .Where(
                    i =>
                        diagram.GetNode(i.Source).ParentNodeId.Equals(containerNodeId) &&
                        diagram.GetNode(i.Target).ParentNodeId.Equals(containerNodeId) &&
                        (connectorFilter != null).Implies(() => connectorFilter.Invoke(i)));

            return LayoutGroup.Create(nodesInLayoutGroup, connectorsInLayoutGroup);
        }
    }
}