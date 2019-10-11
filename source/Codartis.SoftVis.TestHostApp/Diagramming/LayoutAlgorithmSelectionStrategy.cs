using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Diagramming.Definition.Layout;
using Codartis.SoftVis.Diagramming.Implementation.Layout.Vertical;

namespace Codartis.SoftVis.TestHostApp.Diagramming
{
    public sealed class LayoutAlgorithmSelectionStrategy : ILayoutAlgorithmSelectionStrategy
    {
        public IGroupLayoutAlgorithm GetForRoot()
        {
            return new VerticalNodeLayoutAlgorithm();
        }

        public IGroupLayoutAlgorithm GetForNode(IDiagramNode node)
        {
            return new VerticalNodeLayoutAlgorithm();
        }
    }
}
