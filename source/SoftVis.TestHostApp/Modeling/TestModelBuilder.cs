using Codartis.SoftVis.Modeling2;
using Codartis.SoftVis.Modeling2.Implementation;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    internal class TestModelBuilder : ModelBuilderBase
    {
        public IModel AddClass(string name)
        {
            var node = new TestClass(name, name, name, ModelOrigin.SourceCode);
            return AddNode(node);
        }

        public IModel AddInterface(string name)
        {
            var node = new TestInterface(name, name, name, ModelOrigin.SourceCode);
            return AddNode(node);
        }
    }
}
