namespace Codartis.SoftVis.Modeling
{
    /// <summary>
    /// Creates model service instances.
    /// </summary>
    public interface IModelServiceFactory
    {
        IModelService Create();
    }
}
