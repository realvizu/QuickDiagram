using System.Collections.Immutable;
using System.Linq;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.Modeling.Implementation;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    internal sealed class TestModelService : ModelServiceBase, ITestModelService
    {
        public IImmutableList<IImmutableList<IModelNode>> ItemGroups { get; private set; }

        public TestModelService(ModelStore modelStore, IModelRelationshipFactory modelRelationshipFactory)
            : base(modelStore, modelRelationshipFactory)
        {
            ItemGroups = ImmutableList<IImmutableList<IModelNode>>.Empty;
            StartNewGroup();
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