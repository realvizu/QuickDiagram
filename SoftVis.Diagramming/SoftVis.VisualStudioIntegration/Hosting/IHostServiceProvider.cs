namespace Codartis.SoftVis.VisualStudioIntegration.Hosting
{
    public interface IHostServiceProvider
    {
        TServiceInterface GetService<TServiceInterface, TService>()
            where TServiceInterface : class
            where TService : class;
    }
}