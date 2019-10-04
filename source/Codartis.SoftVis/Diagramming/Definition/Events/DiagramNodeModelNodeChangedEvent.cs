using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Definition.Events
{
    public sealed class DiagramNodeModelNodeChangedEvent : DiagramNodeChangedEventBase
    {
        public DiagramNodeModelNodeChangedEvent([NotNull] IDiagramNode oldNode, [NotNull] IDiagramNode newNode)
            : base(oldNode, newNode)
        {
        }

        public override string ToString() => $"{GetType().Name}: {OldNode}";
    }
}