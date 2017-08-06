using Codartis.SoftVis.Modeling2;
using Codartis.SoftVis.Modeling2.Implementation;

namespace Codartis.SoftVis.Diagramming.UnitTests.TestSubjects
{
    internal sealed class TestModelRelationship : ModelRelationship
    {
        public TestModelRelationship(IModelNode source, IModelNode target)
            : base(ModelItemId.Create(),  source, target)
        {
        }

        public TestModelRelationship() 
            : this(null, null)
        {
        }
    }
}
