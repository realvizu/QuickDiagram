using System.Collections.Generic;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling.Definition;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Definition.Layout
{
    /// <summary>
    /// Calculates routes for the given connectors
    /// </summary>
    public interface IConnectorRoutingAlgorithm
    {
        [NotNull]
        IDictionary<ModelRelationshipId, Route> Calculate(
            [NotNull] [ItemNotNull] IEnumerable<IDiagramConnector> connectors,
            [NotNull] IDictionary<ModelNodeId, Rect2D> nodeRects);
    }
}