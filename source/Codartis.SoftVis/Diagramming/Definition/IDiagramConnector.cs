using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Graphs.Immutable;
using Codartis.SoftVis.Modeling.Definition;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Definition
{
    /// <summary>
    /// A diagram connector represents a model relationship and it connects two diagram nodes.
    /// Eg. an inheritance arrow pointing from a derived class shape to its base class shape.
    /// Immutable.
    /// </summary>
    public interface IDiagramConnector : IDiagramShape,
        IImmutableEdge<ModelNodeId, IDiagramConnector, ModelRelationshipId>
    {
        [NotNull] IModelRelationship ModelRelationship { get; }
        ConnectorType ConnectorType { get; }
        Route Route { get; }

        [NotNull]
        IDiagramConnector WithModelRelationship([NotNull] IModelRelationship newModelRelationship);

        [NotNull]
        IDiagramConnector WithConnectorType(ConnectorType newConnectorType);

        [NotNull]
        IDiagramConnector WithRoute(Route newRoute);
    }
}