using System.Windows;

namespace Codartis.SoftVis.Rendering.Wpf.ViewportHandling.Commands
{
    public class MoveViewportCenterInScreenSpaceCommand : ViewportCommandBase
    {
        public Point NewCenterInScreenSpace { get; private set; }

        public MoveViewportCenterInScreenSpaceCommand(Point newCenterInScreenSpace)
        {
            NewCenterInScreenSpace = newCenterInScreenSpace;
        }

        internal override void Execute(Viewport viewport)
        {
            viewport.MoveCenterInScreenSpace(NewCenterInScreenSpace);
        }
    }
}
