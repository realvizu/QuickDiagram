using System;

namespace Codartis.SoftVis.Modeling
{
    /// <summary>
    /// Publishes model change events.
    /// </summary>
    public interface IReadOnlyModelStore
    {
        IModel CurrentModel { get; }

        event Action<ModelEventBase> ModelChanged;
    }
}
