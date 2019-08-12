using System.Collections.Generic;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.Modeling.Implementation;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    public class ContainmentRelationship : ContainmentRelationshipBase
    {
        public ContainmentRelationship(ModelRelationshipId id, IModelNode source, IModelNode target)
            : base(id, source, target)
        {
        }

        protected override IEnumerable<(ModelNodeStereotype, ModelNodeStereotype)> GetValidSourceAndTargetNodeTypePairs()
        {
            yield return (ModelNodeStereotypes.Class, ModelNodeStereotypes.Property);
            yield return (ModelNodeStereotypes.Interface, ModelNodeStereotypes.Property);
        }
    }
}
