namespace Codartis.SoftVis.Util.UI
{
    /// <summary>
    /// Tracks focus changes and assigns a collection of UI decorators to a host UI element.
    /// The decoration can be pinned meaning that it won't follow the focus until unpinned.
    /// </summary>
    /// <typeparam name="THost">The type of the UI element that hosts the decorators.</typeparam>
    public interface IFocusTracker<in THost>
    {
        /// <summary>
        /// Assigns focus to a given host UI element.
        /// </summary>
        /// <param name="host">The host UI element.</param>
        void Focus(THost host);

        /// <summary>
        /// Removes focus from a given host UI element.
        /// </summary>
        /// <param name="host">The host UI element.</param>
        void Unfocus(THost host);

        /// <summary>
        /// Removes focus from all items.
        /// </summary>
        void UnfocusAll();

        /// <summary>
        /// Keeps the decorations visible even when the host loses focus.
        /// </summary>
        void PinDecoration();

        /// <summary>
        /// Exits the "pinned" mode, that is, lets the decorators disappear when the host loses focus.
        /// </summary>
        void UnpinDecoration();
    }
}