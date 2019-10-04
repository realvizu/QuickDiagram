using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Definition.Events
{
    public sealed class DiagramConnectorRemovedEvent : DiagramConnectorEventBase
    {
        [NotNull] public IDiagramConnector OldConnector { get; }

        public DiagramConnectorRemovedEvent([NotNull] IDiagramConnector removedConnector) 
        {
            OldConnector = removedConnector;
        }

        public override string ToString() => $"{GetType().Name}: {OldConnector}";
    }
}