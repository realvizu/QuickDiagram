using System;
using System.Windows;
using System.Windows.Media;

namespace Codartis.Util.UI.Wpf.Controls
{
    /// <summary>
    /// Implemented by those controls that can be the owner of a BubbleListBox.
    /// </summary>
    public interface IBubbleListBoxOwner
    {
        /// <summary>
        /// Fires when the layout of the control is updated. Provided by WPF UIElement class.
        /// </summary>
        event EventHandler LayoutUpdated;

        /// <summary>
        /// Returns a transform that can be used to transform coordinates from this object to the specified visual object.
        /// Provided by WPF Visual class.
        /// </summary>
        /// <param name="visual">The Visual to which the coordinates are transformed.</param>
        /// <returns>A value of type GeneralTransform.</returns>
        GeneralTransform TransformToVisual(Visual visual);

        /// <summary>
        /// Returns the desired handle orientation of the attached bubble list box.
        /// </summary>
        /// <returns>The desired handle orientation for the attached bubble list box.</returns>
        HandleOrientation GetHandleOrientation();

        /// <summary>
        /// Returns a point (relative to the owner control) where the bubble list box handle should be attached.
        /// </summary>
        /// <returns>The desired attach point for the attached bubble list box (in owner control's coordinate system).</returns>
        Point GetAttachPoint();
    }
}
