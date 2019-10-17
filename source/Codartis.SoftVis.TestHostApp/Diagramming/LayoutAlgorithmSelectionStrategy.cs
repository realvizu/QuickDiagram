using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Diagramming.Definition.Layout;
using Codartis.SoftVis.Diagramming.Implementation.Layout.Sugiyama;
using Codartis.SoftVis.Diagramming.Implementation.Layout.Vertical;

namespace Codartis.SoftVis.TestHostApp.Diagramming
{
    public sealed class LayoutAlgorithmSelectionStrategy : ILayoutAlgorithmSelectionStrategy
    {
        public IGroupLayoutAlgorithm GetForRoot()
        {
            return new SugiyamaLayoutAlgorithm(new TestLayoutPriorityProvider());
        }   

        public IGroupLayoutAlgorithm GetForNode(IDiagramNode node)
        {
            return new VerticalNodeLayoutAlgorithm(2);
        }
    }
}
