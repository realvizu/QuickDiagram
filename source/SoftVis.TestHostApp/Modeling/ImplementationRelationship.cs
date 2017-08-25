using System.Collections.Generic;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Modeling.Implementation;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    internal class ImplementationRelationship : ModelRelationship
    {
        public ImplementationRelationship(ModelItemId id, IModelNode source, IModelNode target)
            : base(id, source, target, ModelRelationshipStereotypes.Implementation)
        {
        }

        protected override IEnumerable<(ModelNodeStereotype, ModelNodeStereotype)> GetValidSourceAndTargetNodeTypePairs()
        {
            yield return (ModelNodeStereotype.Class, ModelNodeStereotypes.Interface);
        }
    }
}
