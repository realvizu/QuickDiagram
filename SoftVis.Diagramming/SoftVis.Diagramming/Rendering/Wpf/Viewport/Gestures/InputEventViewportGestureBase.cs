using System.Windows;

namespace Codartis.SoftVis.Rendering.Wpf.Viewport.Gestures
{
    /// <summary>
    /// Base class for gestures that react to UI events.
    /// </summary>
    internal class InputEventViewportGestureBase : ViewportGestureBase
    {
        protected IInputElement InputElement { get; private set; }

        protected InputEventViewportGestureBase(IViewport viewport, IInputElement inputElement)
            : base(viewport)
        {
            InputElement = inputElement;
        }
    }
}
