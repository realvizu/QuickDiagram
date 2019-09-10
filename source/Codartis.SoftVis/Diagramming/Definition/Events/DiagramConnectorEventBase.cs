using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Definition.Events
{
    public abstract class DiagramConnectorEventBase : DiagramEventBase
    {
        protected DiagramConnectorEventBase([NotNull] IDiagram newDiagram)
            : base(newDiagram)
        {
        }
    }
}