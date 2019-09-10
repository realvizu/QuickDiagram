using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Definition.Events
{
    public sealed class DiagramConnectorRouteChangedEvent : DiagramConnectorEventBase
    {
        [NotNull] public IDiagramConnector NewConnector { get; }
        [NotNull] public IDiagramConnector OldConnector { get; }

        public DiagramConnectorRouteChangedEvent(
            [NotNull] IDiagram newDiagram,
            [NotNull] IDiagramConnector oldConnector,
            [NotNull] IDiagramConnector newConnector)
            : base(newDiagram)
        {
            OldConnector = oldConnector;
            NewConnector = newConnector;
        }

        public override string ToString()
            => $"{GetType().Name}: {OldConnector}, {OldConnector.Route}->{NewConnector.Route}";
    }
}