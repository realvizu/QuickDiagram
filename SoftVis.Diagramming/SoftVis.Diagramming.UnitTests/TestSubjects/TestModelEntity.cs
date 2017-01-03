using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Modeling.Implementation;

namespace Codartis.SoftVis.Diagramming.UnitTests.TestSubjects
{
    internal class TestModelEntity : ModelEntity
    {
        public TestModelEntity(string name = null)
            :base(name, name, name, ModelEntityClassifier.Class, ModelEntityStereotype.None, ModelOrigin.Unknown)
        {
        }

        public override int Priority => 0;
    }
}
