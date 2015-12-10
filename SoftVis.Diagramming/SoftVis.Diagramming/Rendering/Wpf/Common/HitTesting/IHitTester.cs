using System.Windows;
using System.Windows.Input;

namespace Codartis.SoftVis.Rendering.Wpf.Common.HitTesting
{
    /// <summary>
    /// Provides hit testing logic.
    /// </summary>
    internal interface IHitTester
    {
        /// <summary>
        /// Returns the topmost visible UI element (if any) that was hit by the mouse event. 
        /// </summary>
        /// <param name="mouseEventArgs">A mouse event the contains the hit point.</param>
        /// <returns>The topmost visible UI element that was hit. Null if none was hit.</returns>
        UIElement HitTest(MouseEventArgs mouseEventArgs);
    }
}
