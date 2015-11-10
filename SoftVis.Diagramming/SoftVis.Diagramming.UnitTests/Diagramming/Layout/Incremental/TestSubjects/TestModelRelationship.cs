using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental.TestSubjects
{
    internal sealed class TestModelRelationship : ModelRelationship
    {
        public TestModelRelationship() 
            : base(null, null, ModelRelationshipType.Generalization)
        {
        }
    }
}
