using System.Collections.Generic;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Diagramming.Definition.Layout;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling.Definition;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Implementation.Layout.DirectConnector
{
    public sealed class DirectConnectorRoutingAlgorithm : IConnectorRoutingAlgorithm
    {
        public IDictionary<ModelRelationshipId, Route> Calculate(
            IEnumerable<IDiagramConnector> connectors,
            IDictionary<ModelNodeId, Rect2D> nodeRects)
        {
            var result = new Dictionary<ModelRelationshipId, Route>();
            foreach (var connector in connectors)
            {
                var route = CreateDirectRoute(connector, nodeRects);
                if (route.HasValue)
                    result.Add(connector.Id, route.Value);
            }

            return result;
        }

        private static Route? CreateDirectRoute(
            [NotNull] IDiagramConnector connector,
            [NotNull] IDictionary<ModelNodeId, Rect2D> nodeRects)
        {
            return nodeRects.TryGetValue(connector.Source, out var sourceRect) &&
                   nodeRects.TryGetValue(connector.Target, out var targetRect) &&
                   sourceRect.Center != targetRect.Center
                ? (Route?)new Route(sourceRect.Center, targetRect.Center)
                    .AttachToSourceRectAndTargetRect(sourceRect, targetRect)
                : null;
        }
    }
}