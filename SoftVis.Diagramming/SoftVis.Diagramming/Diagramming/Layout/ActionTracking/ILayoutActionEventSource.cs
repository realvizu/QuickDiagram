using System;

namespace Codartis.SoftVis.Diagramming.Layout.ActionTracking
{
    /// <summary>
    /// The implementer produces layout action events.
    /// </summary>
    public interface ILayoutActionEventSource
    {
        event EventHandler<ILayoutAction> LayoutActionExecuted;
    }
}