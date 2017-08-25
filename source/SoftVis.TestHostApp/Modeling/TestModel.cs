using System.Collections.Immutable;
using System.Linq;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Modeling.Implementation;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    internal class TestModel : Model
    {
        public ImmutableList<ImmutableList<IModelItem>> ItemGroups { get; }

        public TestModel()
        {
            var newEmptyGroup = ImmutableList.Create<IModelItem>();
            ItemGroups = ImmutableList.Create(newEmptyGroup);
        }

        private TestModel(ModelGraph graph, ImmutableList<ImmutableList<IModelItem>> itemGroups) 
            : base(graph)
        {
            ItemGroups = itemGroups;
        }

        public TestModel AddItemToCurrentGroup(IModelItem modelItem)
        {
            var lastGroup = ItemGroups.Last();
            var updatedItemGroups = ItemGroups.Replace(lastGroup, lastGroup.Add(modelItem));
            return new TestModel(Graph, updatedItemGroups);
        }

        public TestModel StartNewGroup()
        {
            var updatedItemGroups = ItemGroups.Add(ImmutableList.Create<IModelItem>());
            return new TestModel(Graph, updatedItemGroups);
        }

        protected override Model WithGraph(ModelGraph graph)
        {
            return new TestModel(graph, ItemGroups);
        }
    }
}
