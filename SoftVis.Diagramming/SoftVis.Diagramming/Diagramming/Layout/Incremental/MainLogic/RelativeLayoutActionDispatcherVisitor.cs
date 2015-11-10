using Codartis.SoftVis.Diagramming.Layout.Incremental.Relative;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental.MainLogic
{
    internal class RelativeLayoutActionDispatcherVisitor : RelativeLayoutActionVisitorBase
    {
        private readonly IRelativeLayoutChangeConsumer _relativeLayoutChangeConsumer;

        public RelativeLayoutActionDispatcherVisitor(IRelativeLayoutChangeConsumer relativeLayoutChangeConsumer)
        {
            _relativeLayoutChangeConsumer = relativeLayoutChangeConsumer;
        }

        public override void Visit(RelativeVertexAddLayoutAction layoutAction)
        {
            _relativeLayoutChangeConsumer.OnVertexAdded(layoutAction.Vertex, layoutAction.To, layoutAction.CausingLayoutAction);
        }

        public override void Visit(RelativeVertexMoveLayoutAction layoutAction)
        {
            _relativeLayoutChangeConsumer.OnVertexMoved(layoutAction.Vertex, layoutAction.From, layoutAction.To, layoutAction.CausingLayoutAction);
        }
    }
}
