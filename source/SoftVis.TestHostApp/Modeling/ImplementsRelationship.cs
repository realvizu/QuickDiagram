using System;
using System.Collections.Generic;
using Codartis.SoftVis.Modeling2;
using Codartis.SoftVis.Modeling2.Implementation;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    internal class ImplementsRelationship : ModelRelationshipBase
    {
        public ImplementsRelationship(ModelItemId id, IModelNode source, IModelNode target) 
            : base(id, source, target)
        {
        }

        protected override IEnumerable<(Type, Type)> GetValidSourceAndTargetNodeTypePairs()
        {
            yield return (typeof(ClassNode), typeof(InterfaceNode));
        }
    }
}
