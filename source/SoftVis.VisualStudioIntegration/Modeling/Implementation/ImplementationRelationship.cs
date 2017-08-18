using System.Collections.Generic;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Modeling.Implementation;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    /// <summary>
    /// Interface implementation relationship between two model nodes.
    /// </summary>
    internal class ImplementationRelationship : ModelRelationshipBase
    {
        public ImplementationRelationship(ModelItemId id, IModelNode source, IModelNode target) 
            : base(id, source, target, RoslynModelRelationshipStereotype.Implementation)
        {
        }

        protected override IEnumerable<(ModelNodeStereotype, ModelNodeStereotype)> GetValidSourceAndTargetNodeTypePairs()
        {
            yield return (ModelNodeStereotype.Class, RoslynModelNodeStereotype.Interface);
            yield return (RoslynModelNodeStereotype.Struct, RoslynModelNodeStereotype.Interface);
        }
    }
}
