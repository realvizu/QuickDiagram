using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Events
{
    public abstract class DiagramNodeEventBase : DiagramEventBase
    {
        protected DiagramNodeEventBase([NotNull] IDiagram newDiagram)
            : base(newDiagram)
        {
        }
    }
}