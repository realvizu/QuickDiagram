using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Definition.Events
{
    public sealed class DiagramNodeRectChangedEvent : DiagramNodeChangedEventBase
    {
        public DiagramNodeRectChangedEvent([NotNull] IDiagramNode oldNode, [NotNull] IDiagramNode newNode)
            : base(oldNode, newNode)
        {
        }

        public override string ToString() => $"{GetType().Name}: {OldNode}, {OldNode.Rect}->{NewNode.Rect}";
    }
}