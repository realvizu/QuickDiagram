using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Definition.Events
{
    public sealed class DiagramConnectorAddedEvent : DiagramConnectorEventBase
    {
        [NotNull] public IDiagramConnector NewConnector { get; }

        public DiagramConnectorAddedEvent([NotNull] IDiagramConnector newConnector)
        {
            NewConnector = newConnector;
        }

        public override string ToString() => $"{GetType().Name}: {NewConnector}";
    }
}