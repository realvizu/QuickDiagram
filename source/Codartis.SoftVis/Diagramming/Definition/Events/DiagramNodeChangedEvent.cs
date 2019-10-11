using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Definition.Events
{
    public sealed class DiagramNodeChangedEvent : DiagramNodeEventBase
    {
        [NotNull] public IDiagramNode OldNode { get; }
        [NotNull] public IDiagramNode NewNode { get; }
        public DiagramNodeMember ChangedMember { get; }

        public DiagramNodeChangedEvent(
            [NotNull] IDiagramNode oldNode,
            [NotNull] IDiagramNode newNode,
            DiagramNodeMember changedMember)
        {
            OldNode = oldNode;
            NewNode = newNode;
            ChangedMember = changedMember;
        }

        public override string ToString() => $"{nameof(DiagramNodeChangedEvent)}, {ChangedMember}: {OldNode.Id}";
    }
}