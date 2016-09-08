using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Modeling.Implementation;

namespace Codartis.SoftVis.TestHostApp.TestData
{
    internal abstract class TestModelEntity : ModelEntity
    {
        public int Size { get; }

        protected TestModelEntity(string name, int size, ModelEntityClassifier classifier, ModelEntityStereotype stereotype,
            ModelOrigin origin)
            : base(name, "FullName." + name, classifier, stereotype, origin)
        {
            Size = size;
        }
    }
}
