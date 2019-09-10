using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Definition.Events
{
    public sealed class DiagramNodePositionChangedEvent : DiagramNodeChangedEventBase
    {
        public DiagramNodePositionChangedEvent([NotNull] IDiagram newDiagram, [NotNull] IDiagramNode oldNode, [NotNull] IDiagramNode newNode)
            : base(newDiagram, oldNode, newNode)
        {
        }

        public override string ToString() => $"{GetType().Name}: {OldNode}, {OldNode.TopLeft}->{NewNode.TopLeft}";
    }
}