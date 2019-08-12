using System.Collections.Generic;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.Modeling.Implementation;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    internal class AssociationRelationship : ModelRelationship
    {
        public AssociationRelationship(ModelRelationshipId id, IModelNode source, IModelNode target)
            : base(id, source, target, ModelRelationshipStereotypes.Association)
        {
        }

        protected override IEnumerable<(ModelNodeStereotype, ModelNodeStereotype)> GetValidSourceAndTargetNodeTypePairs()
        {
            yield return (ModelNodeStereotypes.Property, ModelNodeStereotypes.Class);
            yield return (ModelNodeStereotypes.Property, ModelNodeStereotypes.Interface);
        }
    }
}
