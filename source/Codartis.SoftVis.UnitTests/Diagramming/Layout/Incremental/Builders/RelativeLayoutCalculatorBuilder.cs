using Codartis.SoftVis.Diagramming.Layout.Incremental;
using Codartis.SoftVis.Diagramming.Layout.Nodes.Layered.Sugiyama;
using Codartis.SoftVis.Diagramming.Layout.Nodes.Layered.Sugiyama.Relative.Logic;

namespace Codartis.SoftVis.UnitTests.Diagramming.Layout.Incremental.Builders
{
    internal class RelativeLayoutCalculatorBuilder : GraphRelatedBuilderBase<DiagramNodeLayoutVertex, LayoutPath, LayeredLayoutGraph>
    {
        public StatefulRelativeLayoutCalculator StatefulRelativeLayoutCalculator { get; }
        public override LayeredLayoutGraph Graph => (LayeredLayoutGraph)StatefulRelativeLayoutCalculator.RelativeLayout.LayeredLayoutGraph;

        public RelativeLayoutCalculatorBuilder()
        {
            StatefulRelativeLayoutCalculator = new StatefulRelativeLayoutCalculator();
        }

        protected override DiagramNodeLayoutVertex CreateVertex(string name, int priority = 1)
        {
            return CreateTestLayoutVertex(name, priority);
        }

        protected override LayoutPath CreateEdge(DiagramNodeLayoutVertex source, DiagramNodeLayoutVertex target)
        {
            return new LayoutPath(source, target, null);
        }

        public override DiagramNodeLayoutVertex AddVertex(string name, int priority = 1)
        {
            var vertex = GetVertex(name);
            if (vertex == null)
            {
                vertex = CreateVertex(name, priority);
                StatefulRelativeLayoutCalculator.OnDiagramNodeAdded(vertex);
            }
            return vertex;
        }

        public override LayoutPath AddEdge(string sourceName, string targetName)
        {
            var edge = GetOrCreateEdge(sourceName, targetName);
            if (edge != null)
                StatefulRelativeLayoutCalculator.OnDiagramConnectorAdded(edge);
            return edge;
        }

        public override DiagramNodeLayoutVertex RemoveVertex(string name)
        {
            var vertex = GetVertex(name);
            if (vertex != null)
                StatefulRelativeLayoutCalculator.OnDiagramNodeRemoved(vertex);
            return vertex;
        }

        public override LayoutPath RemoveEdge(string sourceName, string targetName)
        {
            var edge = GetEdge(sourceName, targetName);
            if (edge != null)
                StatefulRelativeLayoutCalculator.OnDiagramConnectorRemoved(edge);
            return edge;
        }
    }
}
