using System.Collections.Generic;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Modeling.Implementation;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    /// <summary>
    /// Inheritance relationship between two model nodes.
    /// </summary>
    internal class InheritanceRelationship : ModelRelationshipBase
    {
        public InheritanceRelationship(ModelItemId id, IModelNode source, IModelNode target)
            : base(id, source, target, RoslynModelRelationshipStereotype.Implementation)
        {
        }

        protected override IEnumerable<(ModelNodeStereotype, ModelNodeStereotype)> GetValidSourceAndTargetNodeTypePairs()
        {
            yield return (ModelNodeStereotype.Class, ModelNodeStereotype.Class);
            yield return (RoslynModelNodeStereotype.Interface, RoslynModelNodeStereotype.Interface);
        }
    }
}
