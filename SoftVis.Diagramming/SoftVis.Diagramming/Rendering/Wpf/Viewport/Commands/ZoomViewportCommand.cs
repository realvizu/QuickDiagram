namespace Codartis.SoftVis.Rendering.Wpf.Viewport.Commands
{
    public class ZoomViewportCommand : ViewportCommandBase
    {
        public double NewZoom { get; private set; }

        public ZoomViewportCommand(object sender, double newZoom)
            : base(sender)
        {
            NewZoom = newZoom;
        }

        internal override void Execute(IDiagramViewport diagramViewport)
        {
            diagramViewport.ZoomTo(NewZoom);
        }
    }
}
