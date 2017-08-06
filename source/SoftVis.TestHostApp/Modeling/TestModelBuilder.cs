using System.Collections.Immutable;
using Codartis.SoftVis.Modeling2;
using Codartis.SoftVis.Modeling2.Implementation;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    internal class TestModelBuilder : ModelBuilderBase
    {
        public IModel AddClass(string name)
        {
            var node = new TestClass(ModelItemId.Create(),  name, name, name, ModelOrigin.SourceCode, ImmutableList<ImmutableModelNode>.Empty, false);
            return AddNode(node);
        }

        public IModel AddInterface(string name)
        {
            var node = new TestInterface(ModelItemId.Create(), name, name, name, ModelOrigin.SourceCode, ImmutableList<ImmutableModelNode>.Empty, false);
            return AddNode(node);
        }
    }
}
