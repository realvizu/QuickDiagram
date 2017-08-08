using System;
using System.Collections.Generic;
using Codartis.SoftVis.Modeling2;
using Codartis.SoftVis.Modeling2.Implementation;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    internal class TestImplementsRelationship : ModelRelationshipBase
    {
        public TestImplementsRelationship(ModelItemId id, IModelNode source, IModelNode target) 
            : base(id, source, target)
        {
        }

        protected override IEnumerable<(Type, Type)> GetValidSourceAndTargetNodeTypePairs()
        {
            yield return (typeof(TestClass), typeof(TestInterface));
        }
    }
}
