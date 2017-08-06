using Codartis.SoftVis.Modeling2;
using Codartis.SoftVis.Modeling2.Implementation;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    internal class TestImplementsRelationship : ModelRelationship
    {
        public TestImplementsRelationship(ModelItemId id, IModelNode source, IModelNode target) 
            : base(id, source, target)
        {
        }
    }
}
