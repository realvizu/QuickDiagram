using Codartis.SoftVis.Rendering.Common.UIEvents;

namespace Codartis.SoftVis.Rendering.Wpf.Viewport.Gestures
{
    /// <summary>
    /// Base class for gestures that react to UI events.
    /// </summary>
    internal class WidgetEventViewportGestureBase : ViewportGestureBase
    {
        protected IWidgetEventSource WidgetEventSource { get; private set; }

        protected WidgetEventViewportGestureBase(IViewport viewport, IWidgetEventSource widgetEventSource)
            : base(viewport)
        {
            WidgetEventSource = widgetEventSource;
        }
    }
}
