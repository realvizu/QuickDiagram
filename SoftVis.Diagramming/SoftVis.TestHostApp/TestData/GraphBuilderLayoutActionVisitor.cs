using Codartis.SoftVis.Diagramming.Layout;
using Codartis.SoftVis.Diagramming.Layout.ActionGraph;
using Codartis.SoftVis.Diagramming.Layout.BaseActions;

namespace Codartis.SoftVis.TestHostApp.TestData
{
    internal sealed class GraphBuilderLayoutActionVisitor : LayoutActionVisitorBase
    {
        public LayoutActionGraph LayoutActionGraph { get; }

        private readonly static ILayoutAction RootLayoutAction = new LayoutAction("Root");

        public GraphBuilderLayoutActionVisitor()
        {
            LayoutActionGraph = new LayoutActionGraph();
        }

        public void Clear()
        {
            LayoutActionGraph.Clear();
            LayoutActionGraph.AddVertex(RootLayoutAction);
        }

        public override void DefaultVisit(ILayoutAction layoutAction)
        {
            LayoutActionGraph.AddVertex(layoutAction);

            var causingLayoutAction = layoutAction.CausingLayoutAction ?? RootLayoutAction;
            var edge = new LayoutActionEdge(causingLayoutAction, layoutAction);
            LayoutActionGraph.AddEdge(edge);
        }
    }
}
