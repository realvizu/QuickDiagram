using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    /// <summary>
    /// Creates roslyn-based model service instances.
    /// </summary>
    internal class RoslynModelServiceFactory : IModelServiceFactory
    {
        private readonly IRoslynModelProvider _roslynModelProvider;

        public RoslynModelServiceFactory(IRoslynModelProvider roslynModelProvider)
        {
            _roslynModelProvider = roslynModelProvider;
        }

        public IModelService Create()
        {
            return new RoslynModelService(
                new RoslynModelStore(), 
                _roslynModelProvider);
        }
    }
}
