using System.Windows;
using Codartis.SoftVis.Rendering.Common;
using Codartis.SoftVis.Rendering.Wpf.Common;
using Codartis.SoftVis.Rendering.Wpf.ViewportHandling.Commands;

namespace Codartis.SoftVis.Rendering.Wpf.ViewportHandling.Gestures
{
    /// <summary>
    /// Common logic for viewport gestures.
    /// </summary>
    internal abstract class ViewportGestureBase : IViewportGesture
    {
        public event ViewportCommandHandler ViewportCommand;

        public IViewportHost ViewportHost { get; private set; }

        protected ViewportGestureBase(IViewportHost viewportHost)
        {
            ViewportHost = viewportHost;
        }

        protected void ZoomViewportTo(double newZoom)
        {
            SendCommand(new ZoomViewportCommand(newZoom));
        }

        protected void ZoomViewportBy(ZoomDirection zoomDirection, double zoomAmount)
        {
            var newZoom = CalculateZoom(zoomDirection, zoomAmount);
            ZoomViewportTo(newZoom);
        }

        protected void ZoomViewportWithCenterInScreenSpaceTo(double newZoom, Point zoomCenterInScreenSpace)
        {
            SendCommand(new ZoomViewportWithCenterInScreenSpaceCommand(newZoom, zoomCenterInScreenSpace));
        }

        protected void ZoomViewportWithCenterInScreenSpaceBy(ZoomDirection zoomDirection, double zoomAmount, Point zoomCenterInScreenSpace)
        {
            var newZoom = CalculateZoom(zoomDirection, zoomAmount);
            ZoomViewportWithCenterInScreenSpaceTo(newZoom, zoomCenterInScreenSpace);
        }

        private double CalculateZoom(ZoomDirection zoomDirection, double zoomAmount)
        {
            var zoomSign = zoomDirection == ZoomDirection.In ? 1 : -1;

            var linearZoom = ScaleCalculator.ExponentialToLinear(ViewportHost.ViewportZoom, ViewportHost.MinZoom, ViewportHost.MaxZoom);
            var newLinearZoom = linearZoom + (zoomAmount * zoomSign);
            var newZoom = ScaleCalculator.LinearToExponential(newLinearZoom, ViewportHost.MinZoom, ViewportHost.MaxZoom);

            if (newZoom < ViewportHost.MinZoom)
            {
                newZoom = ViewportHost.MinZoom;
            }
            else if (newZoom > ViewportHost.MaxZoom)
            {
                newZoom = ViewportHost.MaxZoom;
            }

            return newZoom;
        }

        protected bool IsZoomLimitReached(ZoomDirection zoomDirection)
        {
            return (zoomDirection == ZoomDirection.In && ViewportHost.ViewportZoom >= ViewportHost.MaxZoom) ||
                  (zoomDirection == ZoomDirection.Out && ViewportHost.ViewportZoom <= ViewportHost.MinZoom);
        }

        protected void MoveViewportCenterInDiagramSpaceBy(Vector vectorInDiagramSpace)
        {
            var newCenterInDiagramSpace = ViewportHost.ViewportInDiagramSpace.GetCenter() - vectorInDiagramSpace;
            MoveViewportCenterInDiagramSpaceTo(newCenterInDiagramSpace);
        }

        protected void MoveViewportCenterInDiagramSpaceTo(Point newCenterInDiagramSpace)
        {
            SendCommand(new MoveViewportCenterInDiagramSpaceCommand(newCenterInDiagramSpace));
        }

        protected void MoveViewportCenterInScreenSpaceBy(Vector vectorInScreenSpace)
        {
            var viewportCenterInScreenSpace = ViewportHost.ViewportInScreenSpace.GetCenter();
            var newCenterInScreenSpace = viewportCenterInScreenSpace + vectorInScreenSpace;
            MoveViewportCenterInScreenSpaceTo(newCenterInScreenSpace);
        }

        protected void MoveViewportCenterInScreenSpaceTo(Point newCenterInScreenSpace)
        {
            SendCommand(new MoveViewportCenterInScreenSpaceCommand(newCenterInScreenSpace));
        }

        protected void ResizeViewportTo(Size newSize)
        {
            SendCommand(new ResizeViewportCommand(newSize));
        }

        private void SendCommand(ViewportCommandBase command)
        {
            if (ViewportCommand != null)
                ViewportCommand(this, command);
        }

        protected enum ZoomDirection
        {
            In,
            Out
        }
    }
}
