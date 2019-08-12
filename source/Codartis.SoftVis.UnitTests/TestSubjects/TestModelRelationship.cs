using System.Collections.Generic;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.Modeling.Implementation;

namespace Codartis.SoftVis.UnitTests.TestSubjects
{
    internal sealed class TestModelRelationship : ModelRelationship
    {
        public TestModelRelationship(IModelNode source, IModelNode target)
            : base(ModelRelationshipId.Create(), source, target, ModelRelationshipStereotype.Containment)
        {
        }

        protected override IEnumerable<(ModelNodeStereotype, ModelNodeStereotype)> GetValidSourceAndTargetNodeTypePairs()
        {
            yield return (TestModelNodeStereotypes.Class, TestModelNodeStereotypes.Class);
        }
    }
}
