using System.Collections.Generic;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Modeling.Implementation;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    /// <summary>
    /// Inheritance relationship between two model nodes.
    /// </summary>
    internal class InheritanceRelationship : ModelRelationship
    {
        public InheritanceRelationship(ModelRelationshipId id, IModelNode source, IModelNode target)
            : base(id, source, target, ModelRelationshipStereotypes.Inheritance)
        {
        }

        protected override IEnumerable<(ModelNodeStereotype, ModelNodeStereotype)> GetValidSourceAndTargetNodeTypePairs()
        {
            yield return (ModelNodeStereotypes.Class, ModelNodeStereotypes.Class);
            yield return (ModelNodeStereotypes.Interface, ModelNodeStereotypes.Interface);
        }
    }
}
