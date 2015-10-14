using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.TestHostApp.TestData
{
    internal abstract class TestModelEntity : ModelEntity
    {
        public int Size { get; }

        protected TestModelEntity(string name, int size, ModelEntityType type, ModelEntityStereotype stereotype = null)
            :base(name, type, stereotype)
        {
            Size = size;
        }
    }
}
