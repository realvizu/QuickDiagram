using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Definition.Events
{
    public sealed class DiagramClearedEvent : DiagramEventBase
    {
        public DiagramClearedEvent([NotNull] IDiagram newDiagram)
            : base(newDiagram)
        {
        }
    }
}