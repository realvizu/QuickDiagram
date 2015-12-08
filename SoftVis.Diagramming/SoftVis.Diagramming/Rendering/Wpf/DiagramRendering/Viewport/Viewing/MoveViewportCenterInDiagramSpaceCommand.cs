using System.Windows;

namespace Codartis.SoftVis.Rendering.Wpf.DiagramRendering.Viewport.Viewing
{
    internal class MoveViewportCenterInDiagramSpaceCommand : ViewportCommandBase
    {
        internal Point NewCenterInDiagramSpace { get; }

        internal MoveViewportCenterInDiagramSpaceCommand(object sender, Point newCenterInDiagramSpace)
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
