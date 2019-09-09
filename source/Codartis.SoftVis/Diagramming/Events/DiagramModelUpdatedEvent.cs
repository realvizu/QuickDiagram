using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Events
{
    public sealed class DiagramModelUpdatedEvent : DiagramEventBase
    {
        public DiagramModelUpdatedEvent([NotNull] IDiagram newDiagram)
            : base(newDiagram)
        {
        }
    }
}