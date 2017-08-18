namespace Codartis.SoftVis.Modeling
{
    /// <summary>
    /// Responsible for modifying a model: adding, updating and removing model items.
    /// The underlying model is immutable so each modification creates a new snapshot of the model.
    /// </summary>
    public interface IModelBuilder : INotifyModelChanged
    {
        IModel CurrentModel { get; }

        IModel ClearModel();
    }
}
