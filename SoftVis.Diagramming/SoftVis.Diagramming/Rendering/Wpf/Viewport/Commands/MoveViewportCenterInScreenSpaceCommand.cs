using System.Windows;

namespace Codartis.SoftVis.Rendering.Wpf.Viewport.Commands
{
    public class MoveViewportCenterInScreenSpaceCommand : ViewportCommandBase
    {
        public Point NewCenterInScreenSpace { get; private set; }

        public MoveViewportCenterInScreenSpaceCommand(object sender, Point newCenterInScreenSpace)
            : base(sender)
        {
            NewCenterInScreenSpace = newCenterInScreenSpace;
        }

        internal override void Execute(IViewport viewport)
        {
            viewport.MoveCenterInScreenSpace(NewCenterInScreenSpace);
        }
    }
}
