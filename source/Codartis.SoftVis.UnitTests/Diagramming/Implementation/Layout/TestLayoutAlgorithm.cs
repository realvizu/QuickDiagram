using Codartis.SoftVis.Diagramming.Definition.Layout;

namespace Codartis.SoftVis.UnitTests.Diagramming.Implementation.Layout
{
    public class TestLayoutAlgorithm : IGroupLayoutAlgorithm
    {
        public LayoutInfo LayoutInfo { get; }

        public TestLayoutAlgorithm(LayoutInfo layoutInfo)
        {
            LayoutInfo = layoutInfo;
        }

        public LayoutInfo Calculate(ILayoutGroup layoutGroup)
        {
            return LayoutInfo;
        }
    }
}