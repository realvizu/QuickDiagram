using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.Modeling.Implementation;
using Codartis.SoftVis.Services;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    internal class TestModelServiceFactory : IModelServiceFactory
    {
        public IModelService Create() => new ModelService();
    }
}