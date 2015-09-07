using System.Windows;

namespace Codartis.SoftVis.Rendering.Wpf.DiagramRendering.Viewport.Commands
{
    public class MoveViewportCenterInDiagramSpaceCommand : ViewportCommandBase
    {
        public Point NewCenterInDiagramSpace { get; private set; }

        public MoveViewportCenterInDiagramSpaceCommand(object sender, Point newCenterInDiagramSpace)
            : base(sender)
        {
            NewCenterInDiagramSpace = newCenterInDiagramSpace;
        }

        internal override void Execute(IDiagramViewport diagramViewport)
        {
            diagramViewport.MoveCenterInDiagramSpace(NewCenterInDiagramSpace);
        }
    }
}
