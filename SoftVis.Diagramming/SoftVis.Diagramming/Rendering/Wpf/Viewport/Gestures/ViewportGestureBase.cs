using System.Windows;
using Codartis.SoftVis.Rendering.Common;
using Codartis.SoftVis.Rendering.Wpf.Common;
using Codartis.SoftVis.Rendering.Wpf.Viewport.Commands;

namespace Codartis.SoftVis.Rendering.Wpf.Viewport.Gestures
{
    /// <summary>
    /// Common logic for viewport gestures.
    /// </summary>
    internal abstract class ViewportGestureBase : IViewportGesture
    {
        public event ViewportCommandHandler ViewportCommand;

        public IViewport Viewport { get; private set; }

        protected ViewportGestureBase(IViewport viewport)
        {
            Viewport = viewport;
        }

        protected void ZoomViewportTo(double newZoom)
        {
            SendCommand(new ZoomViewportCommand(this, newZoom));
        }

        protected void ZoomViewportBy(ZoomDirection zoomDirection, double zoomAmount)
        {
            var newZoom = CalculateZoom(zoomDirection, zoomAmount);
            ZoomViewportTo(newZoom);
        }

        protected void ZoomViewportWithCenterInScreenSpaceTo(double newZoom, Point zoomCenterInScreenSpace)
        {
            SendCommand(new ZoomViewportWithCenterInScreenSpaceCommand(this, newZoom, zoomCenterInScreenSpace));
        }

        protected void ZoomViewportWithCenterInScreenSpaceBy(ZoomDirection zoomDirection, double zoomAmount, Point zoomCenterInScreenSpace)
        {
            var newZoom = CalculateZoom(zoomDirection, zoomAmount);
            ZoomViewportWithCenterInScreenSpaceTo(newZoom, zoomCenterInScreenSpace);
        }

        private double CalculateZoom(ZoomDirection zoomDirection, double zoomAmount)
        {
            var zoomSign = zoomDirection == ZoomDirection.In ? 1 : -1;

            var linearZoom = ScaleCalculator.ExponentialToLinear(Viewport.Zoom, Viewport.MinZoom, Viewport.MaxZoom);
            var newLinearZoom = linearZoom + (zoomAmount * zoomSign);
            var newZoom = ScaleCalculator.LinearToExponential(newLinearZoom, Viewport.MinZoom, Viewport.MaxZoom);

            if (newZoom < Viewport.MinZoom)
            {
                newZoom = Viewport.MinZoom;
            }
            else if (newZoom > Viewport.MaxZoom)
            {
                newZoom = Viewport.MaxZoom;
            }

            return newZoom;
        }

        protected bool IsZoomLimitReached(ZoomDirection zoomDirection)
        {
            return (zoomDirection == ZoomDirection.In && Viewport.Zoom >= Viewport.MaxZoom) ||
                  (zoomDirection == ZoomDirection.Out && Viewport.Zoom <= Viewport.MinZoom);
        }

        protected void MoveViewportCenterInDiagramSpaceBy(Vector vectorInDiagramSpace)
        {
            var newCenterInDiagramSpace = Viewport.ViewportInDiagramSpace.GetCenter() - vectorInDiagramSpace;
            MoveViewportCenterInDiagramSpaceTo(newCenterInDiagramSpace);
        }

        protected void MoveViewportCenterInDiagramSpaceTo(Point newCenterInDiagramSpace)
        {
            SendCommand(new MoveViewportCenterInDiagramSpaceCommand(this, newCenterInDiagramSpace));
        }

        protected void MoveViewportCenterInScreenSpaceBy(Vector vectorInScreenSpace)
        {
            var viewportCenterInScreenSpace = Viewport.ViewportInScreenSpace.GetCenter();
            var newCenterInScreenSpace = viewportCenterInScreenSpace + vectorInScreenSpace;
            MoveViewportCenterInScreenSpaceTo(newCenterInScreenSpace);
        }

        protected void MoveViewportCenterInScreenSpaceTo(Point newCenterInScreenSpace)
        {
            SendCommand(new MoveViewportCenterInScreenSpaceCommand(this, newCenterInScreenSpace));
        }

        protected void ResizeViewportTo(Size newSize)
        {
            SendCommand(new ResizeViewportCommand(this, newSize));
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
