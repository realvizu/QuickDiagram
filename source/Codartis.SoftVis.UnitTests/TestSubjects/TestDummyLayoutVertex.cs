using Codartis.SoftVis.Diagramming.Layout.Nodes.Layered.Sugiyama;

namespace Codartis.SoftVis.UnitTests.TestSubjects
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
