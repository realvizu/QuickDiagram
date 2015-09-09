using Codartis.SoftVis.Rendering.Wpf.DiagramRendering.Viewport.Commands;

namespace Codartis.SoftVis.Rendering.Wpf.DiagramRendering.Viewport.Gestures
{
    /// <summary>
    /// A viewport gesture is a logic that turns input events (mouse, keyboard, control manipulation) 
    /// into viewport commands (zoom and move).
    /// </summary>
    internal interface IViewportGesture
    {
        event ViewportCommandHandler ViewportCommand;

        IDiagramViewport DiagramViewport { get; }
    }
}
