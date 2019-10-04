using System;

namespace Codartis.SoftVis.Modeling.Definition
{
    public interface IModelEventSource
    {
        event Action<ModelEvent> ModelChanged;
    }
}