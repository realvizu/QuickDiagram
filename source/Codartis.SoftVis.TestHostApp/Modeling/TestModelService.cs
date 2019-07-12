using System.Collections.Immutable;
using System.Linq;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Modeling.Implementation;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    internal sealed class TestModelService : ModelServiceBase, ITestModelService
    {
        public IImmutableList<IImmutableList<IModelNode>> ItemGroups { get; private set; }

        public TestModelService()
            : base(new ModelStore(), new TestModelRelationshipFactory())
        {
            ItemGroups = ImmutableList<IImmutableList<IModelNode>>.Empty;
        }

        public void AddItemToCurrentGroup(IModelNode modelItem)
        {
            var lastGroup = ItemGroups.Last();
            ItemGroups = ItemGroups.Replace(lastGroup, lastGroup.Add(modelItem));
        }

        public void StartNewGroup()
        {
            ItemGroups = ItemGroups.Add(ImmutableList.Create<IModelNode>());
        }
    }
}
