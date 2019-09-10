using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Definition.Events
{
    public sealed class DiagramConnectorAddedEvent : DiagramConnectorEventBase
    {
        [NotNull] public IDiagramConnector NewConnector { get; }

        public DiagramConnectorAddedEvent([NotNull] IDiagram newDiagram, [NotNull] IDiagramConnector newConnector)
            : base(newDiagram)
        {
            NewConnector = newConnector;
        }

        public override string ToString() => $"{GetType().Name}: {NewConnector}";
    }
}