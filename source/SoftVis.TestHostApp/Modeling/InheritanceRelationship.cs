using System.Collections.Generic;
using Codartis.SoftVis.Modeling2;
using Codartis.SoftVis.Modeling2.Implementation;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    internal class InheritanceRelationship : ModelRelationshipBase
    {
        public InheritanceRelationship(ModelItemId id, IModelNode source, IModelNode target)
            : base(id, source, target, TestModelRelationshipStereotype.Inheritance)
        {
        }

        protected override IEnumerable<(ModelNodeStereotype, ModelNodeStereotype)> GetValidSourceAndTargetNodeTypePairs()
        {
            yield return (TestModelNodeStereotype.Class, TestModelNodeStereotype.Class);
            yield return (TestModelNodeStereotype.Interface, TestModelNodeStereotype.Interface);
        }
    }
}
