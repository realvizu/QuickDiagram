using System;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Modeling.Definition
{
    /// <summary>
    /// Provides an observable, read-only view of the latest model and its changes.
    /// </summary>
    public interface IModelEventSource
    {
        [NotNull] IModel LatestModel { get; }

        event Action<ModelEvent> ModelChanged;
    }
}