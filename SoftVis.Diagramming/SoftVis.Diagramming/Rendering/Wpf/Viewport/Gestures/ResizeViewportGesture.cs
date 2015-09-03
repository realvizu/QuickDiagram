using System.Windows;
using Codartis.SoftVis.Rendering.Common.UIEvents;

namespace Codartis.SoftVis.Rendering.Wpf.Viewport.Gestures
{
    /// <summary>
    /// Calculates viewport changes when resizing the viewport.
    /// </summary>
    internal class ResizeViewportGesture : WidgetEventViewportGestureBase
    {
        public ResizeViewportGesture(IDiagramViewport diagramViewport, IWidgetEventSource widgetEventSource)
            : base(diagramViewport, widgetEventSource)
        {
            if (WidgetEventSource != null)
                WidgetEventSource.Resize += Resize;
        }

        private void Resize(object sender, ResizeEventArgs args)
        {
            ResizeViewportTo(new Size(args.NewWidth, args.NewHeight));
        }
    }
}
