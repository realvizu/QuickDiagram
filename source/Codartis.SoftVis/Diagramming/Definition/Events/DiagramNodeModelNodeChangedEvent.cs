using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Definition.Events
{
    public sealed class DiagramNodeModelNodeChangedEvent : DiagramNodeChangedEventBase
    {
        public DiagramNodeModelNodeChangedEvent([NotNull] IDiagram newDiagram, [NotNull] IDiagramNode oldNode, [NotNull] IDiagramNode newNode)
            : base(newDiagram, oldNode, newNode)
        {
        }

        public override string ToString() => $"{GetType().Name}: {OldNode}";
    }
}