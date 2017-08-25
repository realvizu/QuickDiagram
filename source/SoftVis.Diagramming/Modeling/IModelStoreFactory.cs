namespace Codartis.SoftVis.Modeling
{
    /// <summary>
    /// Creates model store instances.
    /// </summary>
    public interface IModelStoreFactory
    {
        IModelStore Create();
    }
}
