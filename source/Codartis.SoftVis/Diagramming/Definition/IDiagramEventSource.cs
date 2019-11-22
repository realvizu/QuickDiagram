using System;
using Codartis.SoftVis.Modeling.Definition;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Definition
{
    /// <summary>
    /// Provides an observable, read-only view of the latest diagram and its changes.
    /// </summary>
    public interface IDiagramEventSource
    {
        [NotNull] IDiagram LatestDiagram { get; }

        [NotNull] IObservable<DiagramEvent> DiagramChangedEventStream { get; }

        /// <summary>
        /// Raised whenever the diagram changes.
        /// </summary>
        event Action<DiagramEvent> DiagramChanged;

        /// <summary>
        /// Raised after all handlers of <see cref="DiagramChanged"/> has finished.
        /// </summary>
        /// <remarks>
        /// This is a hack for prioritizing change event handlers.
        /// </remarks>
        event Action<DiagramEvent> AfterDiagramChanged;

        ConnectorType GetConnectorType(ModelRelationshipStereotype stereotype);
    }
}