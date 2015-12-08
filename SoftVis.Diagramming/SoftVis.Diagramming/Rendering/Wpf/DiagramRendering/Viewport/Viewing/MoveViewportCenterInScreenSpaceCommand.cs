using System.Windows;

namespace Codartis.SoftVis.Rendering.Wpf.DiagramRendering.Viewport.Viewing
{
    internal class MoveViewportCenterInScreenSpaceCommand : ViewportCommandBase
    {
        internal Point NewCenterInScreenSpace { get; }

        internal MoveViewportCenterInScreenSpaceCommand(object sender, Point newCenterInScreenSpace)
            : base(sender)
        {
            NewCenterInScreenSpace = newCenterInScreenSpace;
        }

        internal override void Execute(IDiagramViewport diagramViewport)
        {
            diagramViewport.MoveCenterInScreenSpace(NewCenterInScreenSpace);
        }
    }
}
