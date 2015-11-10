using System;

namespace Codartis.SoftVis.Diagramming.Layout
{
    /// <summary>
    /// Abstract base for those classes that publish layout action events.
    /// </summary>
    public abstract class LayoutActionEventSource
    {
        public event EventHandler<ILayoutAction> LayoutActionExecuted;

        protected void RaiseLayoutAction(object sender, ILayoutAction layoutAction)
        {
            LayoutActionExecuted?.Invoke(sender, layoutAction);
        }

        protected void RaiseLayoutAction(ILayoutAction layoutAction)
        {
            RaiseLayoutAction(this, layoutAction);
        }
    }
}