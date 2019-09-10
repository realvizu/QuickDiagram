using Codartis.SoftVis.Diagramming.Layout.Layered.Sugiyama;

namespace Codartis.SoftVis.UnitTests.Diagramming.Layout
{
    internal class TestDummyLayoutVertex : DummyLayoutVertex
    {
        public TestDummyLayoutVertex(int id)
            : base(id)
        {
        }

        public override string Name => $"*{Id}";
        public override string ToString() => Name;
    }
}
