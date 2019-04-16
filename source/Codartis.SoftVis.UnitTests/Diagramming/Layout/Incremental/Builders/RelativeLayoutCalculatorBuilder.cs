using Codartis.SoftVis.Diagramming.Layout.Incremental;
using Codartis.SoftVis.Diagramming.Layout.Incremental.Relative.Logic;

namespace Codartis.SoftVis.UnitTests.Diagramming.Layout.Incremental.Builders
{
    internal class RelativeLayoutCalculatorBuilder : GraphRelatedBuilderBase<DiagramNodeLayoutVertex, LayoutPath, LayeredLayoutGraph>
    {
        public RelativeLayoutCalculator RelativeLayoutCalculator { get; }
        public override LayeredLayoutGraph Graph => (LayeredLayoutGraph)RelativeLayoutCalculator.RelativeLayout.LayeredLayoutGraph;

        public RelativeLayoutCalculatorBuilder()
        {
            RelativeLayoutCalculator = new RelativeLayoutCalculator();
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
                RelativeLayoutCalculator.OnDiagramNodeAdded(vertex);
            }
            return vertex;
        }

        public override LayoutPath AddEdge(string sourceName, string targetName)
        {
            var edge = GetOrCreateEdge(sourceName, targetName);
            if (edge != null)
                RelativeLayoutCalculator.OnDiagramConnectorAdded(edge);
            return edge;
        }

        public override DiagramNodeLayoutVertex RemoveVertex(string name)
        {
            var vertex = GetVertex(name);
            if (vertex != null)
                RelativeLayoutCalculator.OnDiagramNodeRemoved(vertex);
            return vertex;
        }

        public override LayoutPath RemoveEdge(string sourceName, string targetName)
        {
            var edge = GetEdge(sourceName, targetName);
            if (edge != null)
                RelativeLayoutCalculator.OnDiagramConnectorRemoved(edge);
            return edge;
        }
    }
}
