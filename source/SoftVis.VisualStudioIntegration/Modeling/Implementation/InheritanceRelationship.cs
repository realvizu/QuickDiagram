using System;
using System.Collections.Generic;
using Codartis.SoftVis.Modeling2;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    /// <summary>
    /// Inheritance relationship between two model nodes.
    /// </summary>
    internal class InheritanceRelationship : RoslynRelationshipBase, IInheritanceRelationship
    {
        public InheritanceRelationship(ModelItemId id, IModelNode source, IModelNode target)
            : base(id, source, target, RelationshipStereotype.Inheritance)
        {
        }

        protected override IEnumerable<(Type, Type)> GetValidSourceAndTargetNodeTypePairs()
        {
            yield return (typeof(RoslynClassNode), typeof(RoslynClassNode));
            yield return (typeof(RoslynInterfaceNode), typeof(RoslynInterfaceNode));
        }
    }
}
