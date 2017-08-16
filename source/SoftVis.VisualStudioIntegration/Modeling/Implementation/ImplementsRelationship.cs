using System;
using System.Collections.Generic;
using Codartis.SoftVis.Modeling2;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    /// <summary>
    /// Interface implementation relationship between two model nodes.
    /// </summary>
    internal class ImplementsRelationship : RoslynRelationshipBase, IImplementationRelationship
    {
        public ImplementsRelationship(ModelItemId id, IModelNode source, IModelNode target) 
            : base(id, source, target, RelationshipStereotype.Implementation)
        {
        }

        protected override IEnumerable<(Type, Type)> GetValidSourceAndTargetNodeTypePairs()
        {
            yield return (typeof(RoslynClassNode), typeof(RoslynInterfaceNode));
            yield return (typeof(RoslynStructNode), typeof(RoslynInterfaceNode));
        }
    }
}
