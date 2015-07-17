namespace Codartis.SoftVis.Rendering.Wpf.ViewportHandling.Commands
{
    public class ZoomViewportCommand : ViewportCommandBase
    {
        public double NewZoom { get; private set; }

        public ZoomViewportCommand(double newZoom)
        {
            NewZoom = newZoom;
        }

        internal override void Execute(Viewport viewport)
        {
            viewport.ZoomTo(NewZoom);
        }
    }
}
