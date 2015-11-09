using System;

namespace Codartis.SoftVis.Diagramming.Layout.ActionTracking
{
    /// <summary>
    /// Abstract base for those classes that publish layout action events.
    /// </summary>
    public abstract class LayoutActionEventSource : ILayoutActionEventSource
    {
        public event EventHandler<ILayoutAction> LayoutActionExecuted;

        protected void RaiseLayoutAction(object sender, ILayoutAction layoutAction)
        {
            LayoutActionExecuted?.Invoke(this, layoutAction);
        }
    }
}