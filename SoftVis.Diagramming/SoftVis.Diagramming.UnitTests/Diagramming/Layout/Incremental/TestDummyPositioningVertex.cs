using Codartis.SoftVis.Diagramming.Layout.Incremental;

namespace Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental
{
    internal class TestDummyPositioningVertex : DummyPositioningVertex
    {
        public TestDummyPositioningVertex(int id, bool isFloating)
            : base(isFloating)
        {
            Id = id;
        }

        public override string Name => ToString();

        public override string ToString()
        {
            return $"*{Id}";
        }
    }
}
