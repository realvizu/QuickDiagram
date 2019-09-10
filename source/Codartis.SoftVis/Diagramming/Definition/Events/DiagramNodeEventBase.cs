using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Definition.Events
{
    public abstract class DiagramNodeEventBase : DiagramEventBase
    {
        protected DiagramNodeEventBase([NotNull] IDiagram newDiagram)
            : base(newDiagram)
        {
        }
    }
}