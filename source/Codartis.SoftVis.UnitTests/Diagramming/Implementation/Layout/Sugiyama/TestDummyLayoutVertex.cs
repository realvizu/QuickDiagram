using Codartis.SoftVis.Diagramming.Implementation.Layout.Sugiyama;

namespace Codartis.SoftVis.UnitTests.Diagramming.Implementation.Layout.Sugiyama
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
