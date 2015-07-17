using System.Windows;

namespace Codartis.SoftVis.Rendering.Wpf.ViewportHandling.Commands
{
    public class ResizeViewportCommand : ViewportCommandBase
    {
        public Size NewSizeInScreenSpace { get; private set; }

        public ResizeViewportCommand(Size newSizeInScreenSpace)
        {
            NewSizeInScreenSpace = newSizeInScreenSpace;
        }

        internal override void Execute(Viewport viewport)
        {
            viewport.ResizeInScreenSpace(NewSizeInScreenSpace);
        }
    }
}
