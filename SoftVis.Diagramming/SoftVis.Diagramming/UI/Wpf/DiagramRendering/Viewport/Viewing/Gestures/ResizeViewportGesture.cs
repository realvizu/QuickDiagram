using System.Windows;
using Codartis.SoftVis.UI.Common.UIEvents;
using Codartis.SoftVis.UI.Wpf.Common.UIEvents;

namespace Codartis.SoftVis.UI.Wpf.DiagramRendering.Viewport.Viewing.Gestures
{
    /// <summary>
    /// Calculates viewport changes when resizing the viewport.
    /// </summary>
    internal class ResizeViewportGesture : ViewportGestureBase
    {
        internal ResizeViewportGesture(IDiagramViewport diagramViewport, IUIEventSource uiEventSource)
            : base(diagramViewport, uiEventSource)
        {
            UIEventSource.WindowResized += OnWindowResized;
        }

        private void OnWindowResized(object sender, ResizeEventArgs args)
        {
            ResizeViewportTo(new Size(args.NewWidth, args.NewHeight));
        }
    }
}
