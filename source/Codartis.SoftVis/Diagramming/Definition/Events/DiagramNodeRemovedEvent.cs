using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Definition.Events
{
    public sealed class DiagramNodeRemovedEvent : DiagramNodeEventBase
    {
        [NotNull] public IDiagramNode OldNode { get; }

        public DiagramNodeRemovedEvent([NotNull] IDiagramNode removedNode)
        {
            OldNode = removedNode;
        }

        public override string ToString() => $"{GetType().Name}: {OldNode}";
    }
}