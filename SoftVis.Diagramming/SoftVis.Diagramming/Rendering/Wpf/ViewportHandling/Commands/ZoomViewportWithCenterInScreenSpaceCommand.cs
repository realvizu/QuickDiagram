using System.Windows;

namespace Codartis.SoftVis.Rendering.Wpf.ViewportHandling.Commands
{
    public class ZoomViewportWithCenterInScreenSpaceCommand : ViewportCommandBase
    {
        public double NewZoom { get; private set; }
        public Point ZoomCenterInScreenSpace { get; private set; }

        public ZoomViewportWithCenterInScreenSpaceCommand(double newZoom, Point zoomCenterInScreenSpace)
        {
            NewZoom = newZoom;
            ZoomCenterInScreenSpace = zoomCenterInScreenSpace;
        }

        internal override void Execute(Viewport viewport)
        {
            viewport.ZoomWithCenterInScreenSpace(NewZoom, ZoomCenterInScreenSpace);
        }
    }
}
