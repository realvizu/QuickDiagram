namespace Codartis.SoftVis.Modeling
{
    /// <summary>
    /// Provides access to the latest immutable model and fires model change events.
    /// </summary>
    public interface IModelProvider : INotifyModelChanged
    {
        IModel CurrentModel { get; }
    }
}
