using System.Collections.Generic;
using Codartis.SoftVis.Diagramming.Layout.ActionTracking;

namespace Codartis.SoftVis.TestHostApp.TestData
{
    internal sealed class SequenceRecorderLayoutActionVisitor : LayoutActionVisitorBase
    {
        public List<ILayoutAction> LayoutActions { get; }
        public int TotalNodeMoveCount { get; private set; }

        public SequenceRecorderLayoutActionVisitor()
        {
            LayoutActions = new List<ILayoutAction>();
        }

        public void Clear()
        {
            LayoutActions.Clear();
        }

        public override void Visit(IMoveDiagramNodeAction layoutAction)
        {
            RecordLayoutAction(layoutAction);
            TotalNodeMoveCount++;
        }

        public override void Visit(IRerouteDiagramConnectorAction layoutAction)
        {
            RecordLayoutAction(layoutAction);
        }

        private void RecordLayoutAction(ILayoutAction layoutAction)
        {
            LayoutActions.Add(layoutAction);
        }

    }
}
