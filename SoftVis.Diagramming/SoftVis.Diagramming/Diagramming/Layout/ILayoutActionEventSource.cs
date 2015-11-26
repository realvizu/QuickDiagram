using System;

namespace Codartis.SoftVis.Diagramming.Layout
{
    /// <summary>
    /// Publishes layout action events.
    /// </summary>
    public interface ILayoutActionEventSource
    {
        event EventHandler<ILayoutAction> LayoutAction;
    }
}