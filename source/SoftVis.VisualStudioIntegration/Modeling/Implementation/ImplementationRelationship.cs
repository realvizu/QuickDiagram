using System.Collections.Generic;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Modeling.Implementation;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    /// <summary>
    /// Interface implementation relationship between two model nodes.
    /// </summary>
    internal class ImplementationRelationship : ModelRelationship
    {
        public ImplementationRelationship(ModelRelationshipId id, IModelNode source, IModelNode target) 
            : base(id, source, target, ModelRelationshipStereotypes.Implementation)
        {
        }

        protected override IEnumerable<(ModelNodeStereotype, ModelNodeStereotype)> GetValidSourceAndTargetNodeTypePairs()
        {
            yield return (ModelNodeStereotypes.Class, ModelNodeStereotypes.Interface);
            yield return (ModelNodeStereotypes.Struct, ModelNodeStereotypes.Interface);
        }
    }
}
