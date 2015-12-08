namespace Codartis.SoftVis.Rendering.Wpf.DiagramRendering.Viewport.Viewing
{
    internal class ZoomViewportCommand : ViewportCommandBase
    {
        internal double NewZoom { get; private set; }

        internal ZoomViewportCommand(object sender, double newZoom)
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
