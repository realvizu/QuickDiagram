namespace Codartis.SoftVis.VisualStudioIntegration.Hosting
{
    /// <summary>
    /// Operations to access services from the host environment.
    /// </summary>
    internal interface IHostServiceProvider
    {
        TServiceInterface GetService<TServiceInterface, TService>()
            where TServiceInterface : class
            where TService : class;
    }
}