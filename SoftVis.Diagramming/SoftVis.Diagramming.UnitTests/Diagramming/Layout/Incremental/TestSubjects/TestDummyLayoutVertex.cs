using Codartis.SoftVis.Diagramming.Layout.Incremental.Relative;

namespace Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental.TestSubjects
{
    internal class TestDummyLayoutVertex : DummyLayoutVertex
    {
        public TestDummyLayoutVertex(int id, bool isFloating)
            : base(isFloating, id)
        {
        }

        public override string Name => $"*{Id}";
        public override string ToString() => Name;
    }
}
