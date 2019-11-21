namespace Codartis.Util.UI
{
    /// <summary>
    /// Tracks focus changes on UI elements.
    /// </summary>
    /// <typeparam name="TUiElement">The type of the UI element that gets the focus.</typeparam>
    public interface IFocusTracker<in TUiElement>
    {
        /// <summary>
        /// Assigns focus to a given UI element.
        /// </summary>
        void Focus(TUiElement uiElement);

        /// <summary>
        /// Removes focus from a given UI element.
        /// </summary>
        void Unfocus(TUiElement uiElement);

        /// <summary>
        /// Removes focus from all items.
        /// </summary>
        void UnfocusAll();
    }
}