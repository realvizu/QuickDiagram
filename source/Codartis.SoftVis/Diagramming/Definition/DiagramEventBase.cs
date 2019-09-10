using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Definition
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