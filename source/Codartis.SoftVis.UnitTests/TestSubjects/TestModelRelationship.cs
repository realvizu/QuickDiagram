using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.Modeling.Implementation;

namespace Codartis.SoftVis.UnitTests.TestSubjects
{
    internal sealed class TestModelRelationship : ModelRelationship
    {
        public TestModelRelationship(ModelNodeId source, ModelNodeId target)
            : base(ModelRelationshipId.Create(), source, target, ModelRelationshipStereotype.Containment)
        {
        }
    }
}
