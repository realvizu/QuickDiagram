using System;
using System.Collections.Generic;
using Codartis.SoftVis.Modeling2;
using Codartis.SoftVis.Modeling2.Implementation;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    internal class TestInheritanceRelationship : ModelRelationshipBase
    {
        public TestInheritanceRelationship(ModelItemId id, IModelNode source, IModelNode target)
            : base(id, source, target)
        {
        }

        protected override IEnumerable<(Type, Type)> GetValidSourceAndTargetNodeTypePairs()
        {
            yield return (typeof(TestClass), typeof(TestClass));
            yield return (typeof(TestInterface), typeof(TestInterface));
        }
    }
}
