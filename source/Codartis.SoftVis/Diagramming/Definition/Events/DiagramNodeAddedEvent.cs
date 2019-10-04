using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Definition.Events
{
    public sealed class DiagramNodeAddedEvent : DiagramNodeEventBase
    {
        [NotNull] public IDiagramNode NewNode { get; }

        public DiagramNodeAddedEvent([NotNull] IDiagramNode newNode)
        {
            NewNode = newNode;
        }

        public override string ToString() => $"{GetType().Name}: {NewNode}";
    }
}