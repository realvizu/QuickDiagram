using System.Windows;

namespace Codartis.SoftVis.Rendering.Wpf.DiagramRendering.Viewport.Commands
{
    internal class ZoomViewportWithCenterInScreenSpaceCommand : ViewportCommandBase
    {
        internal double NewZoom { get; private set; }
        internal Point ZoomCenterInScreenSpace { get; private set; }

        internal ZoomViewportWithCenterInScreenSpaceCommand(object sender, double newZoom, Point zoomCenterInScreenSpace)
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
