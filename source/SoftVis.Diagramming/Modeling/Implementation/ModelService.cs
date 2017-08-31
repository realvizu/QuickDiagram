namespace Codartis.SoftVis.Modeling.Implementation
{
    /// <summary>
    /// Implements model-related operations.
    /// </summary>
    public class ModelService : IModelService
    {
        public IModelStore ModelStore { get; }

        public ModelService(IModelStore modelStore)
        {
            ModelStore = modelStore;
        }
    }
}
