using System.Windows;

namespace Codartis.SoftVis.Rendering.Wpf.Viewport.Commands
{
    public class ResizeViewportCommand : ViewportCommandBase
    {
        public Size NewSizeInScreenSpace { get; private set; }

        public ResizeViewportCommand(object sender, Size newSizeInScreenSpace)
            : base(sender)
        {
            NewSizeInScreenSpace = newSizeInScreenSpace;
        }

        internal override void Execute(IViewport viewport)
        {
            viewport.ResizeInScreenSpace(NewSizeInScreenSpace);
        }
    }
}
