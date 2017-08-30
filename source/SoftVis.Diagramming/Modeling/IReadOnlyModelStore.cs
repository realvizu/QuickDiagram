using System;

namespace Codartis.SoftVis.Modeling
{
    /// <summary>
    /// Publishes model change events and provides access to the latest model instance.
    /// </summary>
    public interface IReadOnlyModelStore
    {
        IModel CurrentModel { get; }

        event Action<ModelEventBase> ModelChanged;
    }
}
