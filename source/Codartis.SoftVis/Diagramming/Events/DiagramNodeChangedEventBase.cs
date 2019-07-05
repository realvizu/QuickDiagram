using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Events
{
    public abstract class DiagramNodeChangedEventBase : DiagramNodeEventBase
    {
        [NotNull] public IDiagramNode OldNode { get; }
        [NotNull] public IDiagramNode NewNode { get; }

        protected DiagramNodeChangedEventBase([NotNull] IDiagram newDiagram, [NotNull] IDiagramNode oldNode, [NotNull] IDiagramNode newNode)
            : base(newDiagram)
        {
            OldNode = oldNode;
            NewNode = newNode;
        }
    }
}