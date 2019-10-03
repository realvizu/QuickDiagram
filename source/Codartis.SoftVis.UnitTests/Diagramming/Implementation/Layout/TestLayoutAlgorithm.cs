using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Diagramming.Definition.Layout;

namespace Codartis.SoftVis.UnitTests.Diagramming.Implementation.Layout
{
    public class TestLayoutAlgorithm : IGroupLayoutAlgorithm
    {
        public GroupLayoutInfo GroupLayoutInfo { get; }

        public TestLayoutAlgorithm(GroupLayoutInfo groupLayoutInfo)
        {
            GroupLayoutInfo = groupLayoutInfo;
        }

        public GroupLayoutInfo Calculate(ILayoutGroup layoutGroup)
        {
            return GroupLayoutInfo;
        }
    }
}