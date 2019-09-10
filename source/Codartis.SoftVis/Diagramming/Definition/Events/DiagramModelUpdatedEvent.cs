using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Definition.Events
{
    public sealed class DiagramModelUpdatedEvent : DiagramEventBase
    {
        public DiagramModelUpdatedEvent([NotNull] IDiagram newDiagram)
            : base(newDiagram)
        {
        }
    }
}