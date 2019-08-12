namespace Codartis.SoftVis.Modeling.Definition
{
    /// <summary>
    /// Creates model service instances.
    /// </summary>
    public interface IModelServiceFactory
    {
        IModelService Create();
    }
}
