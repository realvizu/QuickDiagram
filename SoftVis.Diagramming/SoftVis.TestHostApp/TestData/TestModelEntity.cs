using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Modeling.Implementation;

namespace Codartis.SoftVis.TestHostApp.TestData
{
    internal abstract class TestModelEntity : ModelEntity
    {
        protected TestModelEntity(string name, ModelEntityClassifier classifier, ModelEntityStereotype stereotype,
            ModelOrigin origin)
            : base(name, "FullName." + name, "Description." + name, classifier, stereotype, origin)
        {
        }
    }
}
