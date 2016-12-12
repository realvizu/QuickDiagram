using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Modeling.Implementation;

namespace Codartis.SoftVis.Diagramming.UnitTests.TestSubjects
{
    internal sealed class TestModelRelationship : ModelRelationship
    {
        public TestModelRelationship(IModelEntity source, IModelEntity target)
            : base(source, target, ModelRelationshipClassifier.Generalization, ModelRelationshipStereotype.None)
        {
        }

        public TestModelRelationship() 
            : this(null, null)
        {
        }
    }
}
