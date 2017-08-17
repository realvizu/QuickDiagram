using System.Collections.Generic;
using Codartis.SoftVis.Modeling2;
using Codartis.SoftVis.Modeling2.Implementation;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    internal class ImplementationRelationship : ModelRelationshipBase
    {
        public ImplementationRelationship(ModelItemId id, IModelNode source, IModelNode target) 
            : base(id, source, target, TestModelRelationshipStereotype.Implementation)
        {
        }

        protected override IEnumerable<(ModelNodeStereotype, ModelNodeStereotype)> GetValidSourceAndTargetNodeTypePairs()
        {
            yield return (ModelNodeStereotype.Class, TestModelNodeStereotype.Interface);
        }
    }
}
