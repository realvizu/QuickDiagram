using System.Windows;
using Codartis.SoftVis.Rendering.Common.UIEvents;

namespace Codartis.SoftVis.Rendering.Wpf.ViewportHandling.Gestures
{
    /// <summary>
    /// Calculates viewport changes when resizing the viewport.
    /// </summary>
    internal class ResizeViewportGesture : UIEventViewportGestureBase
    {
        public ResizeViewportGesture(IViewportHost viewportHost)
            : base(viewportHost)
        {
            if (UIEventSource != null)
                UIEventSource.Resize += Resize;
        }

        private void Resize(object sender, ResizeEventArgs args)
        {
            ResizeViewportTo(new Size(args.NewWidth, args.NewHeight));
        }
    }
}
