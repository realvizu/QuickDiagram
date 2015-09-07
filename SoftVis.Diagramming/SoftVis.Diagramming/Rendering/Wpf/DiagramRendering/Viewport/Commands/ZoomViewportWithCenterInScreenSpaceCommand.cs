using System.Windows;

namespace Codartis.SoftVis.Rendering.Wpf.DiagramRendering.Viewport.Commands
{
    public class ZoomViewportWithCenterInScreenSpaceCommand : ViewportCommandBase
    {
        public double NewZoom { get; private set; }
        public Point ZoomCenterInScreenSpace { get; private set; }

        public ZoomViewportWithCenterInScreenSpaceCommand(object sender, double newZoom, Point zoomCenterInScreenSpace)
            : base(sender)
        {
            NewZoom = newZoom;
            ZoomCenterInScreenSpace = zoomCenterInScreenSpace;
        }

        internal override void Execute(IDiagramViewport diagramViewport)
        {
            diagramViewport.ZoomWithCenterInScreenSpace(NewZoom, ZoomCenterInScreenSpace);
        }
    }
}
