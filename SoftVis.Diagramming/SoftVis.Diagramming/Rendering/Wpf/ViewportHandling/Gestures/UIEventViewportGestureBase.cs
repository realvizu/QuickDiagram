using Codartis.SoftVis.Rendering.Common.UIEvents;

namespace Codartis.SoftVis.Rendering.Wpf.ViewportHandling.Gestures
{
    /// <summary>
    /// Base class for gestures that react to UI events.
    /// </summary>
    internal class UIEventViewportGestureBase : ViewportGestureBase
    {
        protected IUIEventSource UIEventSource { get; private set; }

        protected UIEventViewportGestureBase(IViewportHost viewportHost)
            : base(viewportHost)
        {
            UIEventSource = viewportHost as IUIEventSource;
        }
    }
}
