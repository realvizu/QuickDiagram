using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Events
{
    public abstract class DiagramConnectorEventBase : DiagramEventBase
    {
        protected DiagramConnectorEventBase([NotNull] IDiagram newDiagram)
            : base(newDiagram)
        {
        }
    }
}