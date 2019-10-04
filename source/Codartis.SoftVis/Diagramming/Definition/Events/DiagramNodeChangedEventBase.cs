using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Definition.Events
{
    public abstract class DiagramNodeChangedEventBase : DiagramNodeEventBase
    {
        [NotNull] public IDiagramNode OldNode { get; }
        [NotNull] public IDiagramNode NewNode { get; }

        protected DiagramNodeChangedEventBase([NotNull] IDiagramNode oldNode, [NotNull] IDiagramNode newNode)
        {
            OldNode = oldNode;
            NewNode = newNode;
        }
    }
}