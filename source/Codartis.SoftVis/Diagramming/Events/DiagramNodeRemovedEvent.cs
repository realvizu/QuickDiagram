using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Events
{
    public sealed class DiagramNodeRemovedEvent : DiagramNodeEventBase
    {
        [NotNull] public IDiagramNode OldNode { get; }

        public DiagramNodeRemovedEvent([NotNull] IDiagram newDiagram, [NotNull] IDiagramNode removedNode)
            : base(newDiagram)
        {
            OldNode = removedNode;
        }

        public override string ToString() => $"{GetType().Name}: {OldNode}";
    }
}