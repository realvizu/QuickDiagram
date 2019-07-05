using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Events
{
    public sealed class DiagramConnectorRemovedEvent : DiagramConnectorEventBase
    {
        [NotNull] public IDiagramConnector OldConnector { get; }

        public DiagramConnectorRemovedEvent([NotNull] IDiagram newDiagram, [NotNull] IDiagramConnector removedConnector) 
            : base(newDiagram)
        {
            OldConnector = removedConnector;
        }

        public override string ToString() => $"{GetType().Name}: {OldConnector}";
    }
}