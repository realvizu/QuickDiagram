using System;
using System.Collections.Generic;

namespace Codartis.Util.UI
{
    /// <summary>
    /// Tracks focus and assigns UI decorators to the focused UI element.
    /// The decoration can be pinned meaning that it won't follow the focus until unpinned.
    /// </summary>
    /// <typeparam name="TDecorator">The type of the decorator UI element.</typeparam>
    /// <typeparam name="THost">The type of the UI element that gets the focus and hosts the decorators.</typeparam>
    public interface IDecorationManager<out TDecorator, in THost> : IFocusTracker<THost>, IDisposable
        where TDecorator : IUiDecorator<THost>
    {
        IEnumerable<TDecorator> Decorators { get; }

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