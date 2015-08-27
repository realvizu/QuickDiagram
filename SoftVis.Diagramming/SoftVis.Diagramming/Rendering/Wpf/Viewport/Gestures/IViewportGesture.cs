using Codartis.SoftVis.Rendering.Wpf.Viewport.Commands;

namespace Codartis.SoftVis.Rendering.Wpf.Viewport.Gestures
{
    /// <summary>
    /// A viewport gesture is a logic that turns input events (mouse, keyboard, control manipulation) 
    /// into viewport commands (zoom and move).
    /// </summary>
    public interface IViewportGesture
    {
        event ViewportCommandHandler ViewportCommand;

        IViewport Viewport { get; }
    }
}
