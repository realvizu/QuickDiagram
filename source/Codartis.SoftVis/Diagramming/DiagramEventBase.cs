using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming
{
    public abstract class DiagramEventBase
    {
        [NotNull] public IDiagram NewDiagram { get; }

        protected DiagramEventBase([NotNull] IDiagram newDiagram)
        {
            NewDiagram = newDiagram;
        }
    }
}