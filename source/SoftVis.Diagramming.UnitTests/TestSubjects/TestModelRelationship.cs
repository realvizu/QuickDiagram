using System;
using System.Collections.Generic;
using Codartis.SoftVis.Modeling2;
using Codartis.SoftVis.Modeling2.Implementation;

namespace Codartis.SoftVis.Diagramming.UnitTests.TestSubjects
{
    internal sealed class TestModelRelationship : ModelRelationshipBase
    {
        public TestModelRelationship(IModelNode source, IModelNode target)
            : base(ModelItemId.Create(),  source, target)
        {
        }

        public TestModelRelationship() 
            : this(null, null)
        {
        }

        protected override IEnumerable<(Type, Type)> GetValidSourceAndTargetNodeTypePairs()
        {
            yield return (typeof(TestModelNode), typeof(TestModelNode));
        }
    }
}
