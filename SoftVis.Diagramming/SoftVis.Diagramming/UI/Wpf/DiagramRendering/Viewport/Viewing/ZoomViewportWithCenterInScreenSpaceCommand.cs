using System.Windows;

namespace Codartis.SoftVis.UI.Wpf.DiagramRendering.Viewport.Viewing
{
    internal class ZoomViewportWithCenterInScreenSpaceCommand : ViewportCommandBase
    {
        internal double NewZoom { get; }
        internal Point ZoomCenterInScreenSpace { get; }

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
