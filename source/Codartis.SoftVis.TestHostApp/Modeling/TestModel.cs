using System.Collections.Immutable;
using System.Linq;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Modeling.Implementation;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    internal class TestModel : Model
    {
        public ImmutableList<ImmutableList<IModelNode>> ItemGroups { get; }

        public TestModel()
        {
            var newEmptyGroup = ImmutableList.Create<IModelNode>();
            ItemGroups = ImmutableList.Create(newEmptyGroup);
        }

        private TestModel(ModelGraph graph, ImmutableList<ImmutableList<IModelNode>> itemGroups) 
            : base(graph)
        {
            ItemGroups = itemGroups;
        }

        public TestModel AddItemToCurrentGroup(IModelNode modelItem)
        {
            var lastGroup = ItemGroups.Last();
            var updatedItemGroups = ItemGroups.Replace(lastGroup, lastGroup.Add(modelItem));
            return new TestModel(Graph, updatedItemGroups);
        }

        public TestModel StartNewGroup()
        {
            var updatedItemGroups = ItemGroups.Add(ImmutableList.Create<IModelNode>());
            return new TestModel(Graph, updatedItemGroups);
        }

        protected override IModel CreateInstance(ModelGraph graph)
        {
            return new TestModel(graph, ItemGroups);
        }
    }
}
