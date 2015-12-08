using System.Windows;

namespace Codartis.SoftVis.Rendering.Wpf.DiagramRendering.Viewport.Viewing
{
    internal class ResizeViewportCommand : ViewportCommandBase
    {
        internal Size NewSizeInScreenSpace { get; private set; }

        internal ResizeViewportCommand(object sender, Size newSizeInScreenSpace)
            : base(sender)
        {
            NewSizeInScreenSpace = newSizeInScreenSpace;
        }

        internal override void Execute(IDiagramViewport diagramViewport)
        {
            diagramViewport.ResizeInScreenSpace(NewSizeInScreenSpace);
        }
    }
}
