using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Modeling.Implementation;

namespace Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental.TestSubjects
{
    internal sealed class TestModelRelationship : ModelRelationship
    {
        public TestModelRelationship() 
            : base(null, null, ModelRelationshipClassifier.Generalization, ModelRelationshipStereotype.None)
        {
        }
    }
}
