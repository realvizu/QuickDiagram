using System.Windows;

namespace Codartis.SoftVis.Rendering.Wpf.ViewportHandling.Commands
{
    public class MoveViewportCenterInDiagramSpaceCommand : ViewportCommandBase
    {
        public Point NewCenterInDiagramSpace { get; private set; }

        public MoveViewportCenterInDiagramSpaceCommand(Point newCenterInDiagramSpace)
        {
            NewCenterInDiagramSpace = newCenterInDiagramSpace;
        }

        internal override void Execute(Viewport viewport)
        {
            viewport.MoveCenterInDiagramSpace(NewCenterInDiagramSpace);
        }
    }
}
