using System.Windows;
using Codartis.SoftVis.Rendering.Common;
using Codartis.SoftVis.Rendering.Wpf.Common;
using Codartis.SoftVis.Rendering.Wpf.DiagramRendering.Viewport.Commands;

namespace Codartis.SoftVis.Rendering.Wpf.DiagramRendering.Viewport.Gestures
{
    /// <summary>
    /// Common logic for viewport gestures.
    /// </summary>
    internal abstract class ViewportGestureBase : IViewportGesture
    {
        public event ViewportCommandHandler ViewportCommand;

        public IDiagramViewport DiagramViewport { get; private set; }

        protected ViewportGestureBase(IDiagramViewport diagramViewport)
        {
            DiagramViewport = diagramViewport;
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

            var linearZoom = ScaleCalculator.ExponentialToLinear(DiagramViewport.Zoom, DiagramViewport.MinZoom, DiagramViewport.MaxZoom);
            var newLinearZoom = linearZoom + (zoomAmount * zoomSign);
            var newZoom = ScaleCalculator.LinearToExponential(newLinearZoom, DiagramViewport.MinZoom, DiagramViewport.MaxZoom);

            if (newZoom < DiagramViewport.MinZoom)
            {
                newZoom = DiagramViewport.MinZoom;
            }
            else if (newZoom > DiagramViewport.MaxZoom)
            {
                newZoom = DiagramViewport.MaxZoom;
            }

            return newZoom;
        }

        protected bool IsZoomLimitReached(ZoomDirection zoomDirection)
        {
            return (zoomDirection == ZoomDirection.In && DiagramViewport.Zoom >= DiagramViewport.MaxZoom) ||
                  (zoomDirection == ZoomDirection.Out && DiagramViewport.Zoom <= DiagramViewport.MinZoom);
        }

        protected void MoveViewportCenterInDiagramSpaceBy(Vector vectorInDiagramSpace)
        {
            var newCenterInDiagramSpace = DiagramViewport.ViewportInDiagramSpace.GetCenter() - vectorInDiagramSpace;
            MoveViewportCenterInDiagramSpaceTo(newCenterInDiagramSpace);
        }

        protected void MoveViewportCenterInDiagramSpaceTo(Point newCenterInDiagramSpace)
        {
            SendCommand(new MoveViewportCenterInDiagramSpaceCommand(this, newCenterInDiagramSpace));
        }

        protected void MoveViewportCenterInScreenSpaceBy(Vector vectorInScreenSpace)
        {
            var viewportCenterInScreenSpace = DiagramViewport.ViewportInScreenSpace.GetCenter();
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
